using System;
using System.Collections.Generic;
using Godot;

namespace Wayfarer.Systems.Managers
{
    public class TimeManager : WayfarerNode
    {
        [Signal] public delegate void DayChanged(int day);
        [Signal] public delegate void MonthChanged(string monthString);
        [Signal] public delegate void YearChanged(int year);
        
        private float _baseGameSpeed = 180f; // 3min = 1 day in-game
        
        private static TimeScaleState _currTimeScaleState;
        public static TimeScaleState CurrTimeScaleState => _currTimeScaleState;

        private int _currYear = 1;                    // we don't reset
        private int _currMonth = 1;                   // we reset at 12 in the end of year
        private int _currWeek = 1;                    // we reset at 52 in the end of year
        private int _currDay = 1;                     // we reset at currAmountOfDaysInAMonth in the end of month
        private int _dayOfWeek = 1;

        private readonly string[] _months = new string[12];
        private string _currMonthString = "";
        private readonly int[] _amountOfDaysInAMonth = new int[12];
        private int _currAmountOfDaysInAMonth = 30;

        public override void _Ready ()
        {
            Iterator.Coroutine.Run(KeepRealTimeUpdated());
            
            _amountOfDaysInAMonth[0] = 31;
            _amountOfDaysInAMonth[1] = 28;
            _amountOfDaysInAMonth[2] = 31;
            _amountOfDaysInAMonth[3] = 30;
            _amountOfDaysInAMonth[4] = 31;
            _amountOfDaysInAMonth[5] = 30;
            _amountOfDaysInAMonth[6] = 31;
            _amountOfDaysInAMonth[7] = 31;
            _amountOfDaysInAMonth[8] = 30;
            _amountOfDaysInAMonth[9] = 31;
            _amountOfDaysInAMonth[10] = 30;
            _amountOfDaysInAMonth[11] = 32;

            _months[0] = "Jan";
            _months[1] = "Feb";
            _months[2] = "Mar";
            _months[3] = "Apr";
            _months[4] = "May";
            _months[5] = "Jun";
            _months[6] = "Jul";
            _months[7] = "Aug";
            _months[8] = "Sep";
            _months[9] = "Oct";
            _months[10] = "Nov";
            _months[11] = "Dec";
        }
        
        public void StartGameTimeFromScratch()
        {
            _currMonthString = "Jan";
            
            EmitSignal(nameof(MonthChanged), _currMonthString);
            EmitSignal(nameof(YearChanged), _currYear);
            
            Iterator.Coroutine.Run(KeepGameTimeUpdated());
        }

        public void StartGameTimeFromPreviousState()
        {
            // TODO: When loading a game, load the previous time state
        }

        public void ContinueGameTime()
        {
            Iterator.Coroutine.Run(KeepGameTimeUpdated());
        }

        public IEnumerator<float> KeepGameTimeUpdated()
        {
            while (true)
            {
                if (_currTimeScaleState == TimeScaleState.Pause)
                {
                    yield break;
                }
                
                if (_dayOfWeek == 7)
                {
                    _dayOfWeek = 0;
                    _currWeek++;
                }

                switch (_currMonth)
                {
                    case (1):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[0];
                        break;
                    case (2):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[1];
                        break;
                    case (3):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[2];
                        break;
                    case (4):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[3];
                        break;
                    case (5):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[4];
                        break;
                    case (6):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[5];
                        break;
                    case (7):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[6];
                        break;
                    case (8):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[7];
                        break;
                    case (9):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[8];
                        break;
                    case (10):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[9];
                        break;
                    case (11):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[10];
                        break;
                    case (12):
                        _currAmountOfDaysInAMonth = _amountOfDaysInAMonth[11];
                        break;
                    default:
                        _currAmountOfDaysInAMonth = 30;
                        break;
                }

                if (_currDay == _currAmountOfDaysInAMonth)
                {
                    _currMonth++;
                    _currDay = 0;
                    _currMonthString = _months[_currMonth - 1];
                    EmitSignal(nameof(MonthChanged), _currMonthString);
                }
    
                if (_currMonth == 12 && _currDay == _amountOfDaysInAMonth[11]-1)
                {
                    _currYear++;
                    _currMonth = 1;
                    _currMonthString = "Jan";
                    EmitSignal(nameof(MonthChanged), _currMonthString);
                    _currDay = 0;
                    EmitSignal(nameof(YearChanged), _currYear);
                }
    
                _currDay++;
                _dayOfWeek++;
                EmitSignal(nameof(DayChanged), _currDay);

                if (_currTimeScaleState == TimeScaleState.Fast)
                {
                    yield return _baseGameSpeed / 2;
                }
                else
                {
                    yield return _baseGameSpeed;
                }
            }
        }
        
        public static void ChangeTimeScale(TimeScaleState newState)
        {
            _currTimeScaleState = newState;
        }
        
        
        
        // Below is real time helpers
        
        private static int _msPassed = 0;
        private static int _secondsPassed = 0;
        private static int _minutesPassed = 0;

        public static IEnumerator<float> KeepRealTimeUpdated()
        {
            while (true)
            {
                if (_secondsPassed >= 60)
                {
                    _secondsPassed = 0;
                    _minutesPassed++;
                }

                if (_msPassed >= 1000)
                {
                    _msPassed = 0;
                    _secondsPassed++;
                }

                _msPassed++;
                yield return 0.0001f;
            }
        }

        // TODO: The milliseconds aren't accurate, so none of this is really accurate... Use a stopwatch?

        /// <summary>
        /// Returns milliseconds passed since the application start. NOTE THAT THIS ISN'T REALLY A MILLISECOND RIGHT NOW DUE TO OPTIMIZATION
        /// </summary>
        /// <returns></returns>
        public static float Milliseconds()
        {
            return (_secondsPassed * 1000) + _msPassed;
        }
        
        /// <summary>
        /// Returns seconds passed since the application start. Handy for getting elapsed time (i.e for audio crossfading etc.)
        /// </summary>
        /// <returns></returns>
        public static float Seconds()
        {
            return (_minutesPassed * 60) + _secondsPassed;
        }

        /// <summary>
        /// Returns minutes passed since the application start
        /// </summary>
        /// <returns></returns>
        public static float Minutes()
        {
            return _minutesPassed;
        }
        
        /// <summary>
        /// Returns current time of the day as a floating point number. E.g 9.43 (=09:43) or 21.25 (=21:25)
        /// </summary>
        /// <returns></returns>
        public static float Date()
        {
            return ((float) DateTime.Now.Hour + ((float) DateTime.Now.Minute * 0.01f));
        }
    }
}