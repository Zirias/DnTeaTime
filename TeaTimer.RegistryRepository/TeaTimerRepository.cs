using Microsoft.Win32;
using PalmenIt.dntt.TeaTimer.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PalmenIt.dntt.TeaTimer.RegistryRepository
{
    public class TeaTimerRepository : ITeaTimerRepository
    {
        private class EntryMetaData
        {
            internal int Position { get; set; }
            internal Guid Key { get; }

            public EntryMetaData(Guid key)
            {
                Key = key;
            }

            public EntryMetaData()
            {
                Key = Guid.NewGuid();
            }
        }

        private readonly Dictionary<IEntry<TeaTimerDefinition>, EntryMetaData> _entries;
        private readonly RegistryKey _reg;

        public int Count
        {
            get
            {
                return _entries.Count;
            }
        }

        public TeaTimerRepository()
        {
            _reg = Registry.CurrentUser
                .CreateSubKey("Software")
                .CreateSubKey("PalmenIt")
                .CreateSubKey("DnTeaTime");

            _entries = _reg.GetSubKeyNames()
                .Select(x =>
                {
                    var key = _reg.OpenSubKey(x);
                    try
                    {
                        var def = TeaTimerDefinition.Create(
                            (string)key.GetValue("Name", "<unknown>"),
                            (int)key.GetValue("Minutes", 4),
                            (int)key.GetValue("Seconds", 0));
                        if (def.IsError) return null;
                        IEntry<TeaTimerDefinition> entry = new Entry<TeaTimerDefinition>(def);
                        var meta = new EntryMetaData(Guid.Parse(x));
                        meta.Position = (int)key.GetValue("Pos", 0);
                        return new KeyValuePair<IEntry<TeaTimerDefinition>, EntryMetaData>(entry, meta);
                    }
                    catch
                    {
                        return (KeyValuePair<IEntry<TeaTimerDefinition>, EntryMetaData>?)null;
                    }
                })
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static void WriteRegistryKey(RegistryKey key, string name, int minutes, int seconds, int pos)
        {
            key.SetValue("Name", name, RegistryValueKind.String);
            key.SetValue("Minutes", minutes, RegistryValueKind.DWord);
            key.SetValue("Seconds", seconds, RegistryValueKind.DWord);
            key.SetValue("Pos", pos, RegistryValueKind.DWord);
        }

        public IEntry<TeaTimerDefinition> Add(TeaTimerDefinition value)
        {
            var entry = new Entry<TeaTimerDefinition>(value);
            var pos = _entries.Values.Count > 0 ? _entries.Values.Select(m => m.Position).Max() + 1 : 0;
            _entries.Add(entry, new EntryMetaData() { Position = pos });
            var key = _reg.CreateSubKey(_entries[entry].Key.ToString("B"));
            WriteRegistryKey(key, value.Name, value.Time.Minute, value.Time.Second, pos);
            return entry;
        }

        public IEnumerator<IEntry<TeaTimerDefinition>> GetEnumerator()
        {
            return _entries
                .OrderBy(p => p.Value.Position)
                .Select(p => p.Key)
                .GetEnumerator();
        }

        public int IndexOf(IEntry<TeaTimerDefinition> value)
        {
            return _entries[value].Position;
        }

        public void MoveTo(IEntry<TeaTimerDefinition> value, int position)
        {
            var md = _entries[value];
            if (position == md.Position) return;
            if (position < md.Position)
            {
                var toRenumber = _entries.Values.Where(d => d.Position >= position && d.Position < md.Position).ToList();
                toRenumber.ForEach(d => d.Position += 1);
            }
            else
            {
                var toRenumber = _entries.Values.Where(d => d.Position > md.Position && d.Position <= position).ToList();
                toRenumber.ForEach(d => d.Position -= 1);
            }
            md.Position = position;
            foreach (var entry in _entries.Keys)
            {
                var key = _reg.OpenSubKey(_entries[entry].Key.ToString("B"));
                if (key != null)
                {
                    key.SetValue("Pos", _entries[entry].Position, RegistryValueKind.DWord);
                }
            }
        }

        public void Remove(IEntry<TeaTimerDefinition> value)
        {
            _reg.DeleteSubKeyTree(_entries[value].Key.ToString("B"));
            _entries.Remove(value);
        }

        public void Update(IEntry<TeaTimerDefinition> value)
        {
            var key = _reg.OpenSubKey(_entries[value].Key.ToString());
            if (key != null)
            {
                WriteRegistryKey(key, value.Value.Name,
                    value.Value.Time.Minute, value.Value.Time.Second, _entries[value].Position);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
