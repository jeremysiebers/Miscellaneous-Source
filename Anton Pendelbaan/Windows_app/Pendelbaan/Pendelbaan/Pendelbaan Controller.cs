﻿using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace Pendelbaan
{
    public delegate void ReceivedDataCallback(int Api, UInt16 Value);

    public interface iForm1
    {
        void ReceivedData(int Api, UInt16 Value);
    }

    /*#--------------------------------------------------------------------------#*/
    /*  Description: Main Class
     *
     *  Input(s)   :
     *
     *  Output(s)  :
     *
     *  Returns    :
     *
     *  Pre.Cond.  :
     *
     *  Post.Cond. :
     *
     *  Notes      :
     */
    /*#--------------------------------------------------------------------------#*/
    public partial class Form1 : Form, iForm1
    {
        public SerialPortExample serialPort;
        static System.Windows.Forms.Timer ManPwmTmr = new System.Windows.Forms.Timer();
        public Image tandrad = Pendelbaan.Properties.Resources.tandrad_str_str;
        public int SetPwm;
		public const int TrainGoLeft = 0;
		public const int TrainGoRight = 1;
		public const int On = 1;
		public const int Off = 0;
		public const bool Get = false;
		public const bool Set = true;

        #region Variables
        UInt16 api_size = 0;
        UInt16 watchdog = 0;
        UInt16 train_wait_time = 0;
        UInt16 junction_wait_time = 0;
        UInt16 lights_on_wait_time = 0;
        UInt16 stationary_left = 0;
        UInt16 stationary_right = 0;
        UInt16 max_pwm_right = 0;
        UInt16 max_pwm_left = 0;
        UInt16 max_jerk_pwm_brake = 0;
        UInt16 max_jerk_pwm = 0;
        UInt16 input_debounce = 0;
        UInt16 rc_rb = 0;
        UInt16 rc_rf = 0;
        UInt16 rc_lb = 0;
        UInt16 rc_lf = 0;
        UInt16 btn_start = 0;
        UInt16 btn_stop = 0;
        UInt16 btn_mid = 0;
        UInt16 btn_lb = 0;
        UInt16 btn_lf = 0;
        UInt16 btn_rb = 0;
        UInt16 btn_rf = 0;
        UInt16 train1_pos = 0;
        UInt16 train2_pos = 0;
        UInt16 rc_lmu = 0;
        UInt16 rc_lmd = 0;
        UInt16 rc_rmu = 0;
        UInt16 rc_rmd = 0;
        UInt16 max_pwm_rmu_right = 0;
        UInt16 max_pwm_rmu_left = 0;
        UInt16 max_pwm_rmd_right = 0;
        UInt16 max_pwm_rmd_left = 0;
        UInt16 max_pwm_lmu_right = 0;
        UInt16 max_pwm_lmu_left = 0;
        UInt16 max_pwm_lmd_right = 0;
        UInt16 max_pwm_lmd_left = 0;
        UInt16 delay_rmu_down = 0;
        UInt16 delay_rmu_up = 0;
        UInt16 delay_rmd_down = 0;
        UInt16 delay_rmd_up = 0;
        UInt16 delay_lmd_down = 0;
        UInt16 delay_lmd_up = 0;
        UInt16 delay_lmu_down = 0;
        UInt16 delay_lmu_up = 0;
        UInt16 train_path_from = 0;
        UInt16 train_path_to = 0;
        UInt16 main_program = 0;
        UInt16 junction_left_str = 1;
        UInt16 junction_left_bnd = 0;
        UInt16 junction_right_str = 1;
        UInt16 junction_right_bnd = 0;
        UInt16 actual_pwm_speed = 0;
        UInt16 pwm_brake = 0;
        UInt16 sw_start = 0;
        UInt16 sw_stop = 0;
        UInt16 sw_reset = 0;
        UInt16 sw_junction_left_str = 0;
        UInt16 sw_junction_left_bnd = 0;
        UInt16 sw_junction_right_str = 0;
        UInt16 sw_junction_right_bnd = 0;
        UInt16 sw_pwm_brake_on = 0;
        UInt16 sw_pwm_brake_off = 0;
        UInt16 sw_actual_pwm_speed = 0;
        UInt16 switch_program = 0;
		UInt16 sw_pwm_direction = 0;
        UInt16 junction_left_str_prev = 1;
        UInt16 junction_left_bnd_prev = 0;
        UInt16 junction_right_str_prev = 1;
        UInt16 junction_right_bnd_prev = 0;
        UInt16 pwm_direction = 0;
        #endregion Variables

        #region Indicator init
        public Form1()
        {
            InitializeComponent();

            serialPort = new SerialPortExample("COM4", "\r\n", 900000);


            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports) {
                this.comPoortToolStripMenuItem.Items.Add(port);
            }
            this.comPoortToolStripMenuItem.SelectedItem = this.comPoortToolStripMenuItem.Items[0];


            pictureBox1.Image = Pendelbaan.Properties.Resources.tandrad_str_str;

            ManPwm.Minimum = -255;
            ManPwm.Maximum = 255;            
			ManPwm.MouseLeave += ManPwm_Stop;
            SetPwm = ManPwm.Value;

            MaxPwmRmuLeft.Minimum = 0;
            MaxPwmRmuLeft.Maximum = 255;           

            MaxPwmRmdRight.Minimum = 0;
            MaxPwmRmdRight.Maximum = 255;            

            MaxPwmLmuRight.Minimum = 0;
            MaxPwmLmuRight.Maximum = 255;            

            MaxPwmLmdLeft.Minimum = 0;
            MaxPwmLmdLeft.Maximum = 255;            

            DelayLmuDown.Minimum = 0;
            DelayLmuDown.Maximum = 35000;            

            DelayLmdUp.Minimum = 0;
            DelayLmdUp.Maximum = 35000;            

            DelayRmuDown.Minimum = 0;
            DelayRmuDown.Maximum = 35000;            

            DelayRmdUp.Minimum = 0;
            DelayRmdUp.Maximum = 35000;

            TrainWaitTime.Minimum = 0;
            TrainWaitTime.Maximum = 35000;

            LightsOnWaitTime.Minimum = 0;
            LightsOnWaitTime.Maximum = 35000;

            MaxPwmRight.Minimum = 0;
            MaxPwmRight.Maximum = 255;

            MaxPwmLeft.Minimum = 0;
            MaxPwmLeft.Maximum = 255;

            MaxJerkPwm.Minimum = 0;
            MaxJerkPwm.Maximum = 255;

            MaxJerkPwmBrake.Minimum = 0;
            MaxJerkPwmBrake.Maximum = 255;

            /* Adds the event and the event handler for the method that will 
            process the timer event to the timer. */
            ManPwmTmr.Tick += new EventHandler(ManPwmTmrEventProcessor);
            // Sets the timer interval to 0.1 seconds.
            ManPwmTmr.Interval = 100;
            ManPwmTmr.Start();
        }

        /*#--------------------------------------------------------------------------#*/
        /*  Description: Methods for changing values
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/

        private void DelayRmdUpValueChanged(object sender, EventArgs e)
        {
            delay_rmd_up = Convert.ToUInt16(DelayRmdUp.Value);
            Transceive_Data(Set, API.DELAY_RMD_UP, Convert.ToUInt16(delay_rmd_up / 0.538));
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void DelayRmuDownValueChanged(object sender, EventArgs e)
        {
            delay_rmu_down = Convert.ToUInt16(DelayRmuDown.Value);
            Transceive_Data(Set, API.DELAY_RMU_DOWN, Convert.ToUInt16(delay_rmu_down / 0.538));
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void DelayLmdUpValueChanged(object sender, EventArgs e)
        {
            delay_lmd_up = Convert.ToUInt16(DelayLmdUp.Value);
            Transceive_Data(Set, API.DELAY_LMD_UP, Convert.ToUInt16(delay_lmd_up / 0.538));
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void DelayLmuDownValueChanged(object sender, EventArgs e)
        {
            delay_lmu_down = Convert.ToUInt16(DelayLmuDown.Value);
            Transceive_Data(Set, API.DELAY_LMU_DOWN, Convert.ToUInt16(delay_lmu_down / 0.538));
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxPwmLmdLeftValueChanged(object sender, EventArgs e)
        {
            max_pwm_lmd_left = Convert.ToUInt16(MaxPwmLmdLeft.Value);
            Transceive_Data(Set, API.MAX_PWM_LMD_LEFT, max_pwm_lmd_left);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxPwmLmuRightValueChanged(object sender, EventArgs e)
        {
            max_pwm_lmu_right = Convert.ToUInt16(MaxPwmLmuRight.Value);
            Transceive_Data(Set, API.MAX_PWM_LMU_RIGHT, max_pwm_lmu_right);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxPwmRmdRightValueChanged(object sender, EventArgs e)
        {
            max_pwm_rmd_right = Convert.ToUInt16(MaxPwmRmdRight.Value);
            Transceive_Data(Set, API.MAX_PWM_RMD_RIGHT, max_pwm_rmd_right);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxPwmRmuLeftValueChanged(object sender, EventArgs e)
        {
            max_pwm_rmu_left = Convert.ToUInt16(MaxPwmRmuLeft.Value);
            Transceive_Data(Set, API.MAX_PWM_RMU_LEFT, max_pwm_rmu_left);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void TrainWaitTimeValueChanged(object sender, EventArgs e)
        {
            train_wait_time = Convert.ToUInt16(TrainWaitTime.Value);
            Transceive_Data(Set, API.TRAIN_WAIT_TIME, Convert.ToUInt16(train_wait_time / 0.538));
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void LightsOnWaitTimeValueChanged(object sender, EventArgs e)
        {
            lights_on_wait_time = Convert.ToUInt16(LightsOnWaitTime.Value);
            Transceive_Data(Set, API.LIGHTS_ON_WAIT_TIME, Convert.ToUInt16(lights_on_wait_time / 0.538));
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxJerkPwmBrakeValueChanged(object sender, EventArgs e)
        {
            max_jerk_pwm_brake = Convert.ToUInt16(MaxJerkPwmBrake.Value);
            Transceive_Data(Set, API.MAX_JERK_PWM_BRAKE, max_jerk_pwm_brake);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxJerkPwmValueChanged(object sender, EventArgs e)
        {
            max_jerk_pwm = Convert.ToUInt16(MaxJerkPwm.Value);
            Transceive_Data(Set, API.MAX_JERK_PWM, max_jerk_pwm);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxPwmLeftValueChanged(object sender, EventArgs e)
        {
            max_pwm_left = Convert.ToUInt16(MaxPwmLeft.Value);
            Transceive_Data(Set, API.MAX_PWM_LEFT, max_pwm_left);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        private void MaxPwmRightValueChanged(object sender, EventArgs e)
        {
            max_pwm_right = Convert.ToUInt16(MaxPwmRight.Value);
            Transceive_Data(Set, API.MAX_PWM_RIGHT, max_pwm_right);
            Thread.Sleep(50);
            Transceive_Data(Set, API.SW_EEPROM_STORE, On);
            Thread.Sleep(50);
        }

        /*#--------------------------------------------------------------------------#*/
        /*  Description: private void ManPwm_Stop(object sender, EventArgs e)
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        private void ManPwm_Stop(object sender, EventArgs e)
        {
            if (main_program == 1)
            {
				
				ManPwm.Value = 0;
				SetPwm  = 0;
				Transceive_Data(Set, API.SW_PWM_BRAKE_ON, On);				
				Thread.Sleep(50);
				Transceive_Data(Set, API.SW_ACTUAL_PWM_SPEED, 0); 
				Thread.Sleep(50);
				
            }
        }
		
		/*#--------------------------------------------------------------------------#*/
        /*  Description: private static void ManPwmTmrEventProcessor(Object myObject, EventArgs myEventArgs)
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        private void ManPwmTmrEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            ManPwmTmr.Stop();
			
			if (main_program == 1)
            {
                if(ManPwm.Value > 20 && SetPwm != ManPwm.Value)
                {
                    SetPwm = ManPwm.Value;
					Transceive_Data(Set, API.SW_PWM_BRAKE_OFF, On);
					Thread.Sleep(50);
					Transceive_Data(Set, API.SW_PWM_DIRECTION, TrainGoRight);
					Thread.Sleep(50);
					Transceive_Data(Set, API.SW_ACTUAL_PWM_SPEED, Convert.ToUInt16(ManPwm.Value)); 
					Thread.Sleep(50);					
                }
                else if(ManPwm.Value < -20 && SetPwm != ManPwm.Value)
                {
                    SetPwm = ManPwm.Value;
                    int drive_left = SetPwm * -1;
					
					Transceive_Data(Set, API.SW_PWM_BRAKE_OFF, On);
					Thread.Sleep(50);
					Transceive_Data(Set, API.SW_PWM_DIRECTION, TrainGoLeft);
					Thread.Sleep(50);
					Transceive_Data(Set, API.SW_ACTUAL_PWM_SPEED, Convert.ToUInt16(drive_left)); 
					Thread.Sleep(50);			                    
                }  
				else if(ManPwm.Value > -20 && ManPwm.Value < 20 && SetPwm != ManPwm.Value)
				{
					SetPwm = ManPwm.Value = 0;
					Transceive_Data(Set, API.SW_PWM_BRAKE_ON, On);
					Thread.Sleep(50);					
					Transceive_Data(Set, API.SW_ACTUAL_PWM_SPEED, 0); 
					Thread.Sleep(50);
				}
            }
            ManPwmTmr.Enabled = true;
        }
		
		/*#--------------------------------------------------------------------------#*/
        /*  Description: private void Transceive_Data(bool Set, int API, int Value)
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    : empty strings for set, received data for get
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
		private string Transceive_Data(bool Set, int API, UInt16 Value)
		{
			string DataDir = string.Empty;
			
			if(Set)
			{
				serialPort.Send("s" + "x" + API.ToString() + "x" + Value.ToString() + "G", false);
				return string.Empty;
			}
			else
			{
				return serialPort.Send("g" + "x" + API.ToString() + "G", true);
			}
		}

        /*#--------------------------------------------------------------------------#*/
        /*  Description: Show received character from uController
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        public void ReceivedData(int Api, UInt16 Value)
        {            
            if (SerialRead.InvokeRequired)
            {
                ReceivedDataCallback d = new ReceivedDataCallback(ReceivedData);
                SerialRead.Invoke(d, new object[] { Api, Value });
            }
            else
            {
                progressBar1.Value += 1;
                if (progressBar1.Value > 99)
                {
                    progressBar1.Value = 0;
                }

                SerialRead.AppendText(Api.ToString() + " " + Value.ToString() + Environment.NewLine);

                switch (Api)
                {
                    case API.API_SIZE: api_size = Value; break;
                    case API.WATCHDOG: watchdog = Value; break;

                    case API.TRAIN_WAIT_TIME:
                        train_wait_time = Convert.ToUInt16(Value * 0.538);
                        TrainWaitTime.Value = Convert.ToDecimal(train_wait_time);
                        break;

                    case API.JUNCTION_WAIT_TIME: junction_wait_time = Value; break;

                    case API.LIGHTS_ON_WAIT_TIME:
                        lights_on_wait_time = Convert.ToUInt16(Value * 0.538);
                        LightsOnWaitTime.Value = Convert.ToDecimal(lights_on_wait_time);
                        break;

                    case API.STATIONARY_LEFT: stationary_left = Value; break;
                    case API.STATIONARY_RIGHT: stationary_right = Value; break;

                    case API.MAX_PWM_RIGHT:
                        max_pwm_right = Value;
                        MaxPwmRight.Value = Value;
                        break;

                    case API.MAX_PWM_LEFT:
                        max_pwm_left = Value;
                        MaxPwmLeft.Value = Value;
                        break;

                    case API.MAX_JERK_PWM_BRAKE:
                        max_jerk_pwm_brake = Value;
                        MaxJerkPwmBrake.Value = Value;
                        break;

                    case API.MAX_JERK_PWM:
                        max_jerk_pwm = Value;
                        MaxJerkPwm.Value = Value;
                        break;

                    case API.INPUT_DEBOUNCE: input_debounce = Value; break;

                    case API.RC_RB:
                        rc_rb = Value;
                        if (rc_rb == 1)
                        {
                            RcRb.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcRb.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;

                    case API.RC_RF:
                        rc_rf = Value;
                        if (rc_rf == 1)
                        {
                            RcRf.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcRf.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;

                    case API.RC_LB:
                        rc_lb = Value;
                        if (rc_lb == 1)
                        {
                            RcLb.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcLb.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;

                    case API.RC_LF:
                        rc_lf = Value;
                        if (rc_lf == 1)
                        {
                            RcLf.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcLf.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;

                    case API.BTN_START: btn_start = Value; break;
                    case API.BTN_STOP: btn_stop = Value; break;
                    case API.BTN_MID: btn_mid = Value; break;
                    case API.BTN_LB: btn_lb = Value; break;
                    case API.BTN_LF: btn_lf = Value; break;
                    case API.BTN_RB: btn_rb = Value; break;
                    case API.BTN_RF: btn_rf = Value; break;
                    case API.TRAIN1_POS: train1_pos = Value; break;
                    case API.TRAIN2_POS: train2_pos = Value; break;

                    case API.RC_LMU:
                        rc_lmu = Value;
                        if (rc_lmu == 1)
                        {
                            RcLmu.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcLmu.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;
                    case API.RC_LMD:
                        rc_lmd = Value;
                        if (rc_lmd == 1)
                        {
                            RcLmd.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcLmd.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;
                    case API.RC_RMU:
                        rc_rmu = Value;
                        if (rc_rmu == 1)
                        {
                            RcRmu.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcRmu.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;
                    case API.RC_RMD:
                        rc_rmd = Value;
                        if (rc_rmd == 1)
                        {
                            RcRmd.BackColor = System.Drawing.Color.LimeGreen;
                        }
                        else
                        {
                            RcRmd.BackColor = System.Drawing.Color.LightGray;
                        }
                        break;

                    case API.MAX_PWM_RMU_RIGHT: max_pwm_rmu_right = Value; break; // Not Used

                    case API.MAX_PWM_RMU_LEFT:
                        max_pwm_rmu_left = Value;
                        MaxPwmRmuLeft.Value = Value;
                        break;

                    case API.MAX_PWM_RMD_RIGHT:
                        max_pwm_rmd_right = Value;
                        MaxPwmRmdRight.Value = Value;
                        break;

                    case API.MAX_PWM_RMD_LEFT: max_pwm_rmd_left = Value; break; // Not Used

                    case API.MAX_PWM_LMU_RIGHT:
                        max_pwm_lmu_right = Value;
                        MaxPwmLmuRight.Value = Value;
                        break;

                    case API.MAX_PWM_LMU_LEFT: max_pwm_lmu_left = Value; break; // Not Used
                    case API.MAX_PWM_LMD_RIGHT: max_pwm_lmd_right = Value; break; // Not Used

                    case API.MAX_PWM_LMD_LEFT:
                        max_pwm_lmd_left = Value;
                        MaxPwmLmdLeft.Value = Value;
                        break;

                    case API.DELAY_RMU_DOWN:
                        delay_rmu_down = Convert.ToUInt16(Value * 0.538);
                        DelayRmuDown.Value = Convert.ToDecimal(delay_rmu_down);
                        break;

                    case API.DELAY_RMU_UP: delay_rmu_up = Value; break; // Not Used
                    case API.DELAY_RMD_DOWN: delay_rmd_down = Value; break; // Not Used

                    case API.DELAY_RMD_UP:
                        delay_rmd_up = Convert.ToUInt16(Value * 0.538); ;
                        DelayRmdUp.Value = Convert.ToDecimal(delay_rmd_up);
                        break;

                    case API.DELAY_LMD_DOWN: delay_lmd_down = Value; break; // Not Used

                    case API.DELAY_LMD_UP:
                        delay_lmd_up = Convert.ToUInt16(Value * 0.538); ;
                        DelayLmdUp.Value = Convert.ToDecimal(delay_lmd_up);
                        break;

                    case API.DELAY_LMU_DOWN:
                        delay_lmu_down = Convert.ToUInt16(Value * 0.538); ;
                        DelayLmuDown.Value = Convert.ToDecimal(delay_lmu_down);
                        break;

                    case API.DELAY_LMU_UP: delay_lmu_up = Value; break; // Not Used

                    case API.TRAIN_PATH_FROM: train_path_from = Value; break;

                    case API.TRAIN_PATH_TO: train_path_to = Value; break;

                    case API.MAIN_PROGRAM:
                        main_program = Value;
                        if (main_program == 1)
                        {
                            JunctionLeftBtn.Enabled = true;
                            JunctionRightBtn.Enabled = true;
							ManPwm.Enabled = true;
                        }
                        else if (main_program == 2 || main_program == 3)
                        {
                            JunctionLeftBtn.Enabled = false;
                            JunctionRightBtn.Enabled = false;
							ManPwm.Enabled = false;
                        }
                        break;

                    case API.JUNCTION_LEFT_STR:
                        junction_left_str = Value;
                        if (junction_left_str == 1)
                        {
                            JunctionLeftBtn.Text = "Rechtdoor";                            
                        }
                        break;
                    case API.JUNCTION_LEFT_BND:
                        junction_left_bnd = Value;
                        if (junction_left_bnd == 1)
                        {
                            JunctionLeftBtn.Text = "Afbuigen";                            
                        }
                        break;
                    case API.JUNCTION_RIGHT_STR:
                        junction_right_str = Value;
                        if (junction_right_str == 1)
                        {
                            JunctionRightBtn.Text = "Rechtdoor";                            
                        }
                        break;
                    case API.JUNCTION_RIGHT_BND:
                        junction_right_bnd = Value;
                        if (junction_right_bnd == 1)
                        {
                            JunctionRightBtn.Text = "Afbuigen";                            
                        }
                        break;

                    case API.ACTUAL_PWM_SPEED: actual_pwm_speed = Value; break;
                    case API.PWM_BRAKE: pwm_brake = Value; break;
                    case API.SW_START: sw_start = Value; break;
                    case API.SW_STOP: sw_stop = Value; break;
                    case API.SW_RESET: sw_reset = Value; break;
                    case API.SW_JUNCTION_LEFT_STR: sw_junction_left_str = Value; break;
                    case API.SW_JUNCTION_LEFT_BND: sw_junction_left_bnd = Value; break;
                    case API.SW_JUNCTION_RIGHT_STR: sw_junction_right_str = Value; break;
                    case API.SW_JUNCTION_RIGHT_BND: sw_junction_right_bnd = Value; break;
                    case API.SW_PWM_BRAKE_ON: sw_pwm_brake_on = Value; break;
                    case API.SW_PWM_BRAKE_OFF: sw_pwm_brake_off = Value; break;
                    case API.SW_ACTUAL_PWM_SPEED: sw_actual_pwm_speed = Value; break;
                    case API.SWITCH_PROGRAM: switch_program = Value; break;
					case API.SW_PWM_DIRECTION: sw_pwm_direction = Value; break;

                    case API.JUNCTION_LEFT_STR_PREV:
                        junction_left_str_prev = Value;
                        if (junction_left_str_prev == 1)
                        {
                            junction_left_bnd_prev = 0;
                        }
                        UpdatePicture();
                        break;
                    case API.JUNCTION_LEFT_BND_PREV:
                        junction_left_bnd_prev = Value;
                        if (junction_left_bnd_prev == 1)
                        {
                            junction_left_str_prev = 0;
                        }
                        UpdatePicture();
                        break;
                    case API.JUNCTION_RIGHT_STR_PREV:
                        junction_right_str_prev = Value;
                        if (junction_right_str_prev == 1)
                        {
                            junction_right_bnd_prev = 0;
                        }
                        UpdatePicture();
                        break;
                    case API.JUNCTION_RIGHT_BND_PREV:
                        junction_right_bnd_prev = Value;
                        if (junction_right_bnd_prev == 1)
                        {
                            junction_right_str_prev = 0;
                        }
                        UpdatePicture();
                        break;

                    case API.PWM_DIRECTION: pwm_direction = Value; break;

                    default:
                        break;
                }
            }

        }
		
		/*#--------------------------------------------------------------------------#*/
        /*  Description: private void UpdatePicture()
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        private void UpdatePicture()
        {
            if (junction_left_str_prev == 1 && junction_right_str_prev == 1)
            {
                pictureBox1.Image = Pendelbaan.Properties.Resources.tandrad_str_str;
            }
            else if (junction_left_bnd_prev == 1 && junction_right_bnd_prev == 1)
            {
                pictureBox1.Image = Pendelbaan.Properties.Resources.tandrad_bnd_bnd;
            }
            else if (junction_left_str_prev == 1 && junction_right_bnd_prev == 1)
            {
                pictureBox1.Image = Pendelbaan.Properties.Resources.tandrad_str_bnd;
            }
            else if (junction_left_bnd_prev == 1 && junction_right_str_prev == 1)
            {
                pictureBox1.Image = Pendelbaan.Properties.Resources.tandrad_bnd_str;
            }
        }

        /*#--------------------------------------------------------------------------#*/
        /*  Description: Exit tag in pulldown menu
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Serial.Close();
            this.Close();
        }


        /*#--------------------------------------------------------------------------#*/
        /*  Description: maakVerbindingToolStripMenuItem_Click
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/

        private void maakVerbindingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string port = (string)comPoortToolStripMenuItem.SelectedItem;
            
            serialPort.PortName = port;

            if (maakVerbindingToolStripMenuItem.Text == "Maak Verbinding")
            {   
                try
                {
                    serialPort.Open();
                    comPoortToolStripMenuItem.Enabled = false;
                    maakVerbindingToolStripMenuItem.Enabled = false;
                    NotConnectedBanner.Text = "Ophalen Data";
                    ReadAllData();
                    Task.Factory.StartNew(HardwareReadout);

                    MaxPwmRmuLeft.ValueChanged += MaxPwmRmuLeftValueChanged;
                    MaxPwmRmdRight.ValueChanged += MaxPwmRmdRightValueChanged;
                    MaxPwmLmuRight.ValueChanged += MaxPwmLmuRightValueChanged;
                    MaxPwmLmdLeft.ValueChanged += MaxPwmLmdLeftValueChanged;
                    DelayLmuDown.ValueChanged += DelayLmuDownValueChanged;
                    DelayLmdUp.ValueChanged += DelayLmdUpValueChanged;
                    DelayRmuDown.ValueChanged += DelayRmuDownValueChanged;
                    DelayRmdUp.ValueChanged += DelayRmdUpValueChanged;
                    MaxPwmRight.ValueChanged += MaxPwmRightValueChanged;
                    MaxPwmLeft.ValueChanged += MaxPwmLeftValueChanged;
                    MaxJerkPwm.ValueChanged += MaxJerkPwmValueChanged;
                    MaxJerkPwmBrake.ValueChanged += MaxJerkPwmBrakeValueChanged;
                    LightsOnWaitTime.ValueChanged += LightsOnWaitTimeValueChanged;
                    TrainWaitTime.ValueChanged += TrainWaitTimeValueChanged;
                    NotConnectedBanner.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij openen van " + (string)comPoortToolStripMenuItem.SelectedItem);
                }
            }            
        }
                
        /*#--------------------------------------------------------------------------#*/
        /*  Description: private void HardwareReadout()
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        private void HardwareReadout()
        {
            try
            {
                while (true)
                {
                    RawData(serialPort.Read());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Receiver error");
            }
        }

        /*#--------------------------------------------------------------------------#*/
        /*  Description: private void ReadAllData()
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        private void ReadAllData()
        {
			RawData(Transceive_Data(Get, API.API_SIZE, 0));
			
            for (int i = API.API_SIZE + 1; i < api_size; i++)
            {
                RawData(Transceive_Data(Get, i, 0));
                Thread.Sleep(50);
            }
            
        }
        #endregion Indicator init

        /*#--------------------------------------------------------------------------#*/
        /*  Description: private async Task startread(CancellationToken ct)
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        public void RawData(string tString)
        {

            if (tString != string.Empty && tString != null)
            {
                //tString += Encoding.ASCII.GetString(received);
                //Console.WriteLine(tString);
                //Console.WriteLine("-------------------------------------------------------------------------");


                if (tString.IndexOf("M#") == 0)
                {
                    UInt16 j = 0;
                    UInt16 Api = 0;
                    UInt16 Value = 0;
                    string[] numbers = Regex.Split(tString, @"\D+");
                    foreach (string value in numbers)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            UInt16 i = UInt16.Parse(value);
                            //Console.WriteLine("Number: {0}", i);
                            if (j == 0)
                            {
                                Api = i;
                                j = 1;
                            }
                            else if (j == 1)
                            {
                                Value = i;
                                j = 2;
                            }
                        }
                    }

                    if (Api != 0)
                    {
                        ReceivedData(Api, Value);
                    }

                }

                tString = "";
            }

        }

        /*#--------------------------------------------------------------------------#*/
        /*  Description: Buttons
         *
         *  Input(s)   :
         *
         *  Output(s)  :
         *
         *  Returns    :
         *
         *  Pre.Cond.  :
         *
         *  Post.Cond. :
         *
         *  Notes      :
         */
        /*#--------------------------------------------------------------------------#*/
        private void JunctionLeftBtn_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if (JunctionLeftBtn.Text == "Rechtdoor")
                {
					Transceive_Data(Set, API.SW_JUNCTION_LEFT_BND, On);                    
                    JunctionLeftBtn.Text = "Afbuigen";
                }
                else
                {
					Transceive_Data(Set, API.SW_JUNCTION_LEFT_STR, On);
                    JunctionLeftBtn.Text = "Rechtdoor";
                }
            }          
        }

        private void JunctionRightBtn_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if (JunctionRightBtn.Text == "Rechtdoor")
                {
					Transceive_Data(Set, API.SW_JUNCTION_RIGHT_BND, On);
                    JunctionRightBtn.Text = "Afbuigen";
                }
                else
                {
					Transceive_Data(Set, API.SW_JUNCTION_RIGHT_STR, On);
                    JunctionRightBtn.Text = "Rechtdoor";
                }
            }
        }
    }



    /*#--------------------------------------------------------------------------#*/
    /*  Description: static class API
     *
     *  Input(s)   :
     *
     *  Output(s)  :
     *
     *  Returns    :
     *
     *  Pre.Cond.  :
     *
     *  Post.Cond. :
     *
     *  Notes      :
     */
    /*#--------------------------------------------------------------------------#*/
    static class API
    {
        public const int API_SIZE = 16;
        public const int WATCHDOG = 17;
        public const int TRAIN_WAIT_TIME = 18;
        public const int JUNCTION_WAIT_TIME = 19;
        public const int LIGHTS_ON_WAIT_TIME = 20;
        public const int STATIONARY_LEFT = 21;
        public const int STATIONARY_RIGHT = 22;
        public const int MAX_PWM_RIGHT = 23;
        public const int MAX_PWM_LEFT = 24;
        public const int MAX_JERK_PWM_BRAKE = 25;
        public const int MAX_JERK_PWM = 26;
        public const int INPUT_DEBOUNCE = 27;
        public const int RC_RB = 28;
        public const int RC_RF = 29;
        public const int RC_LB = 30;
        public const int RC_LF = 31;
        public const int BTN_START = 32;
        public const int BTN_STOP = 33;
        public const int BTN_MID = 34;
        public const int BTN_LB = 35;
        public const int BTN_LF = 36;
        public const int BTN_RB = 37;
        public const int BTN_RF = 38;
        public const int TRAIN1_POS = 39;
        public const int TRAIN2_POS = 40;
        public const int RC_LMU = 41;
        public const int RC_LMD = 42;
        public const int RC_RMU = 43;
        public const int RC_RMD = 44;
        public const int MAX_PWM_RMU_RIGHT = 45;
        public const int MAX_PWM_RMU_LEFT = 46;
        public const int MAX_PWM_RMD_RIGHT = 47;
        public const int MAX_PWM_RMD_LEFT = 48;
        public const int MAX_PWM_LMU_RIGHT = 49;
        public const int MAX_PWM_LMU_LEFT = 50;
        public const int MAX_PWM_LMD_RIGHT = 51;
        public const int MAX_PWM_LMD_LEFT = 52;
        public const int DELAY_RMU_DOWN = 53;
        public const int DELAY_RMU_UP = 54;
        public const int DELAY_RMD_DOWN = 55;
        public const int DELAY_RMD_UP = 56;
        public const int DELAY_LMD_DOWN = 57;
        public const int DELAY_LMD_UP = 58;
        public const int DELAY_LMU_DOWN = 59;
        public const int DELAY_LMU_UP = 60;
        public const int TRAIN_PATH_FROM = 61;
        public const int TRAIN_PATH_TO = 62;
        public const int MAIN_PROGRAM = 63;
        public const int JUNCTION_LEFT_STR = 64;
        public const int JUNCTION_LEFT_BND = 65;
        public const int JUNCTION_RIGHT_STR = 66;
        public const int JUNCTION_RIGHT_BND = 67;
        public const int ACTUAL_PWM_SPEED = 68;
        public const int PWM_BRAKE = 69;
        public const int SW_START = 70;
        public const int SW_STOP = 71;
        public const int SW_RESET = 72;
        public const int SW_JUNCTION_LEFT_STR = 73;
        public const int SW_JUNCTION_LEFT_BND = 74;
        public const int SW_JUNCTION_RIGHT_STR = 75;
        public const int SW_JUNCTION_RIGHT_BND = 76;
        public const int SW_PWM_BRAKE_ON = 77;
        public const int SW_PWM_BRAKE_OFF = 78;
        public const int SW_ACTUAL_PWM_SPEED = 79;
        public const int SWITCH_PROGRAM = 80;
		public const int SW_PWM_DIRECTION = 81;
        public const int JUNCTION_LEFT_STR_PREV = 82;
        public const int JUNCTION_LEFT_BND_PREV = 83;
        public const int JUNCTION_RIGHT_STR_PREV = 84;
        public const int JUNCTION_RIGHT_BND_PREV = 85;
        public const int PWM_DIRECTION = 86;	
		public const int SW_EEPROM_STORE = 87;
    }
}