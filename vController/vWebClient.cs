using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Web;
using System.Threading;

namespace vControler
{
    public class vMixWebClient
    {
        WebClient vMix;

        public string URL { get { return vMix.BaseAddress; } set { vMix.BaseAddress = value; } }
        public List<vMixInput> vMixInputs;
        List<string> ForceToStop;

        public vMixWebClient(string baseadress)
        {
            vMix = new WebClient();
            vMix.BaseAddress = baseadress;
            vMixInputs = new List<vMixInput>();
        }

        private int FindOverlay(XmlDocument doc , int number) 
        {
            int result = 0;
            foreach (XmlNode node in doc.SelectNodes("vmix/overlays/overlay"))
            {
                if (node.InnerText != "")
                {
                    if (int.Parse(node.InnerText) == number) { result = int.Parse(node.Attributes.GetNamedItem("number").Value); }
                }
            }
            return result;
        }

        public bool GetStatus()
        {
            vMixInputs.Clear();
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(vMix.DownloadString("api"));
            }
            catch 
            {
                return false; 
            }

            string[] v = doc.SelectNodes("vmix/version")[0].InnerText.Split('.');

            if (int.Parse(v[0]) < 11)
                return false;

            foreach (XmlNode node in doc.SelectNodes("vmix/inputs/input"))
            {
                vMixInput vmi = new vMixInput();
                vmi.guid = node.Attributes.GetNamedItem("key").Value;
                vmi.number = int.Parse(node.Attributes.GetNamedItem("number").Value);
                vmi.type = node.Attributes.GetNamedItem("type").Value;
                vmi.state = node.Attributes.GetNamedItem("state").Value;
                vmi.position = int.Parse(node.Attributes.GetNamedItem("position").Value);
                vmi.duration = int.Parse(node.Attributes.GetNamedItem("duration").Value);
                try
                {
                    vmi.muted = bool.Parse(node.Attributes.GetNamedItem("muted").Value);
                }
                catch 
                {
                    vmi.muted = false;
                }
                vmi.loop = bool.Parse(node.Attributes.GetNamedItem("loop").Value);
                vmi.name = node.InnerText;
                vmi.overlay = FindOverlay(doc, vmi.number);
                vMixInputs.Add(vmi);
            }
            return true;     
        }

        public bool GetGUID(string inputname, out string guid)
        {
            guid = "invalid";

            if (!GetStatus())
                return false;

            foreach (vMixInput vmi in vMixInputs)
            {
                if (vmi.name == inputname)
                {
                    guid = vmi.guid;
                    break;
                }
            }

            return true;
        }

        public int FindNumber(string guid)
        {
            if (!GetStatus())
                return 0;

            foreach (vMixInput vmi in vMixInputs)
            {
                if (vmi.guid == guid)
                {
                    return vmi.number;
                }
            }
            return 0;
        }

        public string BooltoString(bool toconvert, string True="On", string False="Off")
        {
            if (toconvert) return True;
            else return False;
        }
        public List<string> FindMembers( string overlay)
        {
            List<string> result = new List<string>();
            if (GetStatus())
            {
                foreach (vMixInput vme in vMixInputs)
                {
                    if (vme.overlay == int.Parse(overlay)) result.Add(vme.guid);
                }
            }
            return result;
        }

        public bool AddInput(string type, string overlay, string path, string guid)
        {
            try
            {
                ForceToStop = FindMembers(overlay);
                string link = "api?function=AddInput&Input=" + HttpUtility.UrlEncode(guid) + "&Value=" + type + "|" + HttpUtility.UrlEncode(path);
                vMix.DownloadString(link);
                vMix.DownloadString("api?function=AutoPauseOn&Input=" + guid);
                vMix.DownloadString("api?function=AutoPlayOn&Input=" + guid);
                vMix.DownloadString("api?function=AudioAutoOff&Input=" + guid);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool MuteAudio(bool audiostate, string guid)
        {
            try
            {
                string link = "api?function=Audio" + BooltoString(!audiostate) + "&Input=" + Convert.ToString(FindNumber(guid));
                vMix.DownloadString(link);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SetupSlideshow(int intervall, string transitioneffect, int transitiontime, string guid)
        {
            try
            {
                vMix.DownloadString("api?function=SetPictureTransition&Input=" + HttpUtility.UrlEncode(guid) + "&Value=" + intervall.ToString());
                vMix.DownloadString("api?function=SetPictureEffect&Input=" + HttpUtility.UrlEncode(guid) + "&Value=" + transitioneffect);
                vMix.DownloadString("api?function=SetPictureEffectDuration&Input=" + HttpUtility.UrlEncode(guid) + "&Value=" + transitiontime.ToString());
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool ForwardTo(string guid, int position)
        {
            try
            {
                vMix.DownloadString("api?function=SetPosition&Value=" + position.ToString() + "&Input=" + HttpUtility.UrlEncode(guid));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Transition(string guid, string overlay, bool audiostate, string type, int duration)
        {
            try
            {
                //vMix.DownloadString("api?function=Play&Input=" + HttpUtility.UrlEncode(guid));
                //foreach (string input in ForceToStop)
                //{
                //    MuteAudio(true, input);
                //}
                if (overlay == "0")
                {
                    MuteAudio(audiostate, guid);
                    vMix.DownloadString("api?function=" + type + "&Duration=" + duration.ToString() + "&Input=" + guid);
                }
                else
                {
                    MuteAudio(audiostate, guid);
                    vMix.DownloadString("api?function=OverlayInput" + overlay + "&input=" + Convert.ToString(FindNumber(guid)));
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool NextPicture(string guid)
        {
            try
            {
                vMix.DownloadString("api?function=NextPicture&Input=" + HttpUtility.UrlEncode(guid));
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool CloseInput(string guid)
        {
            try
            {
                vMix.DownloadString("api?function=RemoveInput&Input=" + HttpUtility.UrlEncode(guid));
            }
            catch
            {
                return false;
            }
            return true;
        }
    }

    public struct vMixInput
    {
        public int number;
        public string guid;
        public string type;
        public string state;
        public int position;
        public int duration;
        public bool muted;
        public bool loop;
        public string name;
        public int overlay;
    }
}