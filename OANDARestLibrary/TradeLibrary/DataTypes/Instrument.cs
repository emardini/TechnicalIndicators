namespace OANDARestLibrary.TradeLibrary.DataTypes
{
    public class Instrument
    {
        #region Fields

        public bool HasInstrument;

        public bool HasdisplayName;

        [IsOptional]
        public bool HasextraPrecision;

        public bool HasmaxTradeUnits;

        public bool Haspip;

        [IsOptional]
        public bool HaspipLocation;

        private string _displayName;

        private int _extraPrecision;

        private string _instrument;

        private int _maxTradeUnits;

        private string _pip;

        private int _pipLocation;

        #endregion

        #region Public Properties

        public string displayName
        {
            get { return this._displayName; }
            set
            {
                this._displayName = value;
                this.HasdisplayName = true;
            }
        }
        public int extraPrecision
        {
            get { return this._extraPrecision; }
            set
            {
                this._extraPrecision = value;
                this.HasextraPrecision = true;
            }
        }
        public string instrument
        {
            get { return this._instrument; }
            set
            {
                this._instrument = value;
                this.HasInstrument = true;
            }
        }

        public int maxTradeUnits
        {
            get { return this._maxTradeUnits; }
            set
            {
                this._maxTradeUnits = value;
                this.HasmaxTradeUnits = true;
            }
        }
        public string pip
        {
            get { return this._pip; }
            set
            {
                this._pip = value;
                this.Haspip = true;
            }
        }
        public int pipLocation
        {
            get { return this._pipLocation; }
            set
            {
                this._pipLocation = value;
                this.HaspipLocation = true;
            }
        }

        #endregion
    }
}