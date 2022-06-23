using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using NationalInstruments;
using NationalInstruments.NI4882;
using NationalInstruments.VisaNS;

namespace CommonLibrary.Measurement
{
    public class GpibInstrument
    {
        private MessageBasedSession mbSession;
        private MessageBasedSessionReader mbsr;
        private MessageBasedSessionWriter mbsw;
        private short _gpibAdrs;
        public short gpibAdrs { get { return _gpibAdrs; } }

        //コンストラクタ
        public GpibInstrument(short gpibAddress)
        {
            this._gpibAdrs = gpibAddress;
        }

        //タイムアウト時間の取得
        public int getTimeout()
        {
            return this.mbSession.Timeout;
        }

        //タイムアウト時間の設定
        public void setTimeout(int timeout)
        {
            this.mbSession.Timeout = timeout;
        }

        //リモート
        public void remote()
        {
            try
            {
                string[] resourceNames; 
                ResourceManager rm;
                rm = ResourceManager.GetLocalManager();
                resourceNames = rm.FindResources("GPIB0::" + this._gpibAdrs + "::INSTR");
                mbSession = new MessageBasedSession(resourceNames[0]);

                new GpibSession(mbSession.ResourceName).ControlRen(RenMode.Assert);//リモートへ移行
                
                this.mbsr = new MessageBasedSessionReader(this.mbSession);
                this.mbsr.DefaultStringSize = 65536;
                this.mbsw = new MessageBasedSessionWriter(this.mbSession);
            }
            
            catch (Exception)
            {
                MessageBox.Show("GPIB機器との接続が確認できません。接続を確認して下さい。", "エラー", MessageBoxButtons.OK);
            }
        }

        //ローカル
        public void local() 
        {
            try
            {
                new GpibSession(mbSession.ResourceName).ControlRen(RenMode.Deassert); //ローカルへ移行
                mbSession.Terminate();
                mbSession.Dispose();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("GPIB機器との接続が確認できません。接続を確認して下さい。", "エラー", MessageBoxButtons.OK);
            }
        }

        //クエリ(送受信)
        public string query(string strMessage)
        {
            try
            {
                return this.mbSession.Query(strMessage);
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        //送信
        public void send(string strMessage)
        {
            try
            {
                //mbSession.Write(strMessage);
                this.mbsw.Write(strMessage);
                this.mbsw.Flush();
            }
            catch (NullReferenceException) 
            {
                MessageBox.Show("GPIB機器との接続が確認できません。接続を確認して下さい。", "エラー", MessageBoxButtons.OK);
            }


            catch (Exception e) { throw e; }
        }

        //受信(stringバッファサイズ指定)
        public string receive(int readerStringSize)
        {
            try
            {
                this.mbsr.DefaultStringSize = readerStringSize;
                return this.mbsr.ReadToEnd();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        //受信(stringバッファサイズ指定なし)
        public string receive()
        {
            try
            {
                return this.mbsr.ReadToEnd();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

    }
}
