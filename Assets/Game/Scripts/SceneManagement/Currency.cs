using System;

namespace SceneManagement
{
    public class Currency
    {
        private double _count;

        public event Action<double> OnChangeCount;
        
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
            IGame.Instance.saveGame.MakeSave();
            OnChangeCount?.Invoke(_count);
        }
    }
}