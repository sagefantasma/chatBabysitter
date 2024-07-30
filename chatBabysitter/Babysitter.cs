namespace chatBabysitter
{
    public class Babysitter
    {
        public enum BabysittingModes
        {
            ClipShow,
            Ai,
            InteractiveGame,
            Trivia
        }

        #region Fields & Properties
        private bool _currentlyEntertaining = false;
        public bool CurrentlyEntertaining
        {
            get { return _currentlyEntertaining; }
            set
            {
                if (value == false)
                {
                    StopEntertaining();
                }
                else
                {
                    StartEntertaining();
                }
                _currentlyEntertaining = value;
            }
        }
        private static bool IsActive { get; set; } = false;
        private bool AfkDetection { get; set; } = false;
        private uint AfkThresholdMs { get; set; } = 120000;
        #endregion

        public void ActivateBabysitter(bool startTheEntertainment = false)
        {
            IsActive = true;
            CurrentlyEntertaining = startTheEntertainment;
        }

        public void DeactivateBabysitter()
        {
            IsActive = false;
            CurrentlyEntertaining = false;
        }

        private void StartEntertaining()
        {
            
        }

        private void StopEntertaining()
        {

        }

        public void SetAfkThreshold(uint thresholdMs)
        {
            AfkThresholdMs = thresholdMs;
        }
    }
}
