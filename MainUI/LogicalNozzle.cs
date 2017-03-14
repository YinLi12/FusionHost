using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainUI
{
    public class LogicalNozzle : INotifyPropertyChanged
    {
        public enum PumpNozzleState { Idle, BusyCardInserted, BusyLiftedOrFueling }

        private PumpNozzleState _State;

        public PumpNozzleState NozzleState
        {
            get
            {
                return this._State;
            }
            set
            {
                if (this._State != value)
                {
                    this._State = value;
                    var safe = this.PropertyChanged;
                    safe?.Invoke(this, new PropertyChangedEventArgs("NozzleState"));
                }
            }
        }

        public byte NozzleNumber
        { get; set; }

        private int _VolumnAccumulator;
        public int VolumnAccumulator
        {
            get { return this._VolumnAccumulator; }
            set
            {
                this._VolumnAccumulator = value;
                var safe = this.PropertyChanged;
                safe?.Invoke(this, new PropertyChangedEventArgs("VolumnAccumulator"));
            }
        }

        #region 卡插入中, 相关的额外信息

        public string InsertedCardNumber { get; set; }
        public string InsertedCardStateCode { get; set; }
        public int InsertedCardBalance { get; set; }

        #endregion

        #region 抬枪或加油中, 相关的额外信息

        public int Amount { get; set; }
        public int Volumn { get; set; }
        public int Price { get; set; }

        #endregion

        /// <summary>
        /// reset all current state, used in nozzle state changing.
        /// </summary>
        public void ResetState()
        {
            this.Amount = 0;
            this.Volumn = 0;
            // should earse this?
            this.Price = 0;
            this.InsertedCardNumber = null;
            this.InsertedCardStateCode = null;
            this.InsertedCardBalance = 0;
            this.NozzleState = PumpNozzleState.Idle;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
