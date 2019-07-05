using System.Collections.Generic;

namespace ProjectRenaissance
{
    public sealed class ChipPool
    {
        List<Chip> _chips = new List<Chip>();
        List<Chip> _1000s = new List<Chip>();
        List<Chip> _500s = new List<Chip>();
        List<Chip> _250s = new List<Chip>();
        List<Chip> _50s = new List<Chip>();
        List<Chip> _10s = new List<Chip>();
        List<Chip> _1s = new List<Chip>();

        public int Total { get; private set; }

        public void Add(Chip chip)
        {
            _chips.Add(chip);
            Total += chip.Value;

            switch (chip.Value)
            {
                case 1000:
                    _1000s.Add(chip);
                    break;
                case 500:
                    _500s.Add(chip);
                    break;
                case 250:
                    _250s.Add(chip);
                    break;
                case 50:
                    _50s.Add(chip);
                    break;
                case 10:
                    _10s.Add(chip);
                    break;
                case 1:
                    _1s.Add(chip);
                    break;
                default:
                    break;
            }
        }
        public void AddRange(List<Chip> chips)
        {
            foreach (Chip chip in chips)
                Add(chip);
        }
        public bool Remove(Chip chip)
        {
            if (_chips.Remove(chip))
            {
                switch (chip.Value)
                {
                    case 1000:
                        _1000s.Remove(chip);
                        break;
                    case 500:
                        _500s.Remove(chip);
                        break;
                    case 250:
                        _250s.Remove(chip);
                        break;
                    case 50:
                        _50s.Remove(chip);
                        break;
                    case 10:
                        _10s.Remove(chip);
                        break;
                    case 1:
                        _1s.Remove(chip);
                        break;
                }
                Total -= chip.Value;
                return true;
            }
            return false;
        }
        public void Clear()
        {
            _chips.Clear();
            Total = 0;
        }
        public void Lock()
        {
            for (int i = 0; i < _chips.Count; i++)
            {
                _chips[i].IsLocked = true;
            }
        }
        public void Unlock()
        {
            for (int i = 0; i < _chips.Count; i++)
            {
                _chips[i].IsLocked = false;
            }
        }
        public List<Chip> Take(int amount)
        {
            List<Chip> chips = new List<Chip>();

            while (amount > 0)
            {
                Chip chip = null;

                if (amount >= 1000 && _1000s.Count > 0)
                {
                    chip = _1000s[0];
                    _1000s.RemoveAt(0);
                    amount -= 1000;
                }
                else if (amount >= 500 && _500s.Count > 0)
                {
                    chip = _500s[0];
                    _500s.RemoveAt(0);
                    amount -= 500;
                }
                else if (amount >= 250 && _250s.Count > 0)
                {
                    chip = _250s[0];
                    _250s.RemoveAt(0);
                    amount -= 250;
                }
                else if (amount >= 50 && _50s.Count > 0)
                {
                    chip = _50s[0];
                    _50s.RemoveAt(0);
                    amount -= 50;
                }
                else if (amount >= 10 && _10s.Count > 0)
                {
                    chip = _10s[0];
                    _10s.RemoveAt(0);
                    amount -= 10;
                }
                else if (amount >= 1 && _1s.Count > 0)
                {
                    chip = _1s[0];
                    _1s.RemoveAt(0);
                    amount--;
                }
                else
                {
                    break;
                }

                chips.Add(chip);
                _chips.Remove(chip);
            }

            if (amount > 0)
            {
                if (amount < 10 && _10s.Count > 0)
                    _10s[0].Breakdown(false);
                else if (amount < 50 && _50s.Count > 0)
                    _50s[0].Breakdown(false);
                else if (amount < 250 && _250s.Count > 0)
                    _250s[0].Breakdown(false);
                else if (amount < 500 && _500s.Count > 0)
                    _500s[0].Breakdown(false);
                else if (amount < 1000 && _1000s.Count > 0)
                    _1000s[0].Breakdown(false);

                chips.AddRange(Take(amount));
            }

            return chips;
        }

        public List<Chip> GetChips()
        {
            List<Chip> deepCopy = new List<Chip>();

            foreach (Chip chip in _chips)
                deepCopy.Add(chip);

            return deepCopy;
        }
    }
}