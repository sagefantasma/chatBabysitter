namespace chatBabysitter
{
    public class Babysitter
    {
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

        public void ActivateBabysitter(bool startTheEntertainment)
        {
            IsActive = true;
            CurrentlyEntertaining = startTheEntertainment;
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
