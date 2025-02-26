using System;
using Saving;

namespace SceneManagement
{
    public class Currency
    {
        private double _count;

        private SaveGame _saveGame;
        
        public event Action<double> OnChangeCount;

        public Currency(SaveGame saveGame)
        {
            _saveGame = saveGame;
        }
        
        public double Count // TODO not used code
        {
            get { return _count; }
        }

        public double OnLoadCount  // TODO not used code
        {
            set
            {
                _count = value;
                OnChangeCount?.Invoke(_count);
            }
        }

        public void ChangeCount(double change) // TODO change to Increase, Decrease
        {
            _count += change;
            OnChangeCount?.Invoke(_count);
        }

        public void SetCount(double count) // TODO change to Increase, Decrease
        {
            _count = count;
            _saveGame.MakeSave();
            OnChangeCount?.Invoke(_count);
        }
    }
}