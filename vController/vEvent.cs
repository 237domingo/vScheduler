using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace vControler
{
    public enum vmEventType {manual, black, video, audio, image, photos, input};
    public enum vmTransitionType {cut, fade, zoom, wipe, slide, fly, cross, rotate, cube, cubezoom};

    public class vMixEvent
    {
        public string GUID; 
        public string Title;
        public vmEventType EventType;
        public DateTime EventStart;
        public bool EventLooping = false;
        public int state = 0;
        public vmTransitionType EventTransition = vmTransitionType.fade;
        public int EventTransitionTime = 500;
        public TimeSpan EventInPoint = new TimeSpan(0);
        public TimeSpan MediaDuration;
        public TimeSpan EventDuration;
        public DateTime EventEnd
        {
            get
            {
                return EventStart + EventDuration;
            }
            set
            {
                EventDuration = value - (EventStart + EventInPoint);
            }
        }
        //
        // returns 0 for no media, 1 for folder and 2 for file
        public int MediaType
        {
            get
            {
                switch (EventType)
                {
                    case vmEventType.black:
                        return 0;
                    case vmEventType.video:
                        return 3;
                    case vmEventType.audio:
                        return 3;
                    case vmEventType.image:
                        return 2;
                    case vmEventType.photos:
                        return 1;
                    case vmEventType.input:
                        return 0;
                    default:
                        return 0;
                }
            }
        }
        public bool HasDuration { get { return EventType == vmEventType.video || EventType == vmEventType.audio; } }
        public bool HasMedia { get { return EventType != vmEventType.manual; } }
        public bool IsCurrent { get { return DateTime.Now > EventStart && DateTime.Now < EventEnd; } }
        public bool IsLoaded { get { return state > 0; } }
        public bool IsRunning { get { return state > 1; } }
        public bool IsLike(vMixEvent other)
        {
            if (EventStart != other.EventStart) return false;
            if (EventPath != other.EventPath) return false;
            if (Overlay != other.Overlay) return false;
            return true;
        }
        public string EventPath;
        public int SlideshowInterval = 5;
        public vmTransitionType SlideshowTransition = vmTransitionType.fade;
        public int SlideshowTransitionTime = 500;
        public bool EventMuted = false;
        public string Overlay;
       
        public string EventTypeString()
        {
            switch (EventType)
            {
                case vmEventType.black:
                    return "colour";
                case vmEventType.video:
                    return "video";
                case vmEventType.audio:
                    return "audio";
                case vmEventType.image:
                    return "image";
                case vmEventType.photos:
                    return "photos";
                case vmEventType.input:
                    return "input";
                default:
                    return "manual";
            }
        }
        public vmEventType EventTypeFromString(string type)
        {
            switch (type.ToLower ())
            {
                case "black":
                    return vmEventType.black;
                case "colour":
                    return vmEventType.black;
                case "color":
                    return vmEventType.black;
                case "video":
                    return vmEventType.video;
                case "audio":
                    return vmEventType.audio;
                case "image":
                    return vmEventType.image;
                case "photos":
                    return vmEventType.photos;
                case "input":
                    return vmEventType.input;
                default:
                    return vmEventType.manual;
            }
        }
        public string TransitionTypeString()
        {
            return TransitionTypeString(EventTransition);
        }
        public string SlideshowTypeString()
        {
            return TransitionTypeString(SlideshowTransition);
        }
        public string TransitionTypeString(vmTransitionType trans)
        {
            switch (trans)
            {
                case vmTransitionType.fade:
                    return "Fade";
                case vmTransitionType.zoom:
                    return "Zoom";
                case vmTransitionType.wipe:
                    return "Wipe";
                case vmTransitionType.slide:
                    return "Slide";
                case vmTransitionType.fly:
                    return "Fly";
                case vmTransitionType.cross:
                    return "CrossZoom";
                case vmTransitionType.rotate:
                    return "FlyRotate";
                case vmTransitionType.cube:
                    return "Cube";
                case vmTransitionType.cubezoom:
                    return "CubeZoom";
                default:
                    return "Cut";
            }
        }
        public vmTransitionType TransitionTypeFromString(string type)
        {
            switch (type)
            {
                case "Fade":
                    return vmTransitionType.fade;
                case "Zoom":
                    return vmTransitionType.zoom;
                case "Wipe":
                    return vmTransitionType.wipe;
                case "Slide":
                    return vmTransitionType.slide;
                case "Fly":
                    return vmTransitionType.fly;
                case "CrossZoom":
                    return vmTransitionType.cross;
                case "FlyRotate":
                    return vmTransitionType.rotate;
                case "Cube":
                    return vmTransitionType.cube;
                case "CubeZoom":
                    return vmTransitionType.cubezoom;
                default:
                    return vmTransitionType.cut;
            }
        }

        public vMixEvent()
        {
 
        }

        public vMixEvent(vMixEvent e)
        {
            state = e.state;
            Title = e.Title;
            GUID = e.GUID;
            EventPath = e.EventPath;
            EventType = e.EventType;
            EventStart = e.EventStart;
            EventInPoint = e.EventInPoint;
            MediaDuration = e.MediaDuration;
            EventDuration = e.EventDuration;
            EventTransition = e.EventTransition;
            EventTransitionTime = e.EventTransitionTime;
            EventLooping = e.EventLooping;
            EventMuted = e.EventMuted;
            SlideshowInterval = e.SlideshowInterval;
            SlideshowTransition = e.SlideshowTransition;
            SlideshowTransitionTime = e.SlideshowTransitionTime;
            Overlay = e.Overlay;
        }

        public vMixEvent(XmlNode node)
        {
            state = 0;
            GUID = System.Guid.NewGuid().ToString();
            Title = node.Attributes.GetNamedItem("Title").Value;
            EventPath = node.Attributes.GetNamedItem("Path").Value;
            EventType = EventTypeFromString(node.Attributes.GetNamedItem("Type").Value);
            EventStart = DateTime.Parse(node.Attributes.GetNamedItem("Start").Value);
            EventDuration = TimeSpan.Parse(node.Attributes.GetNamedItem("EventDuration").Value);
            if (HasDuration)
            {
                EventInPoint = TimeSpan.Parse(node.Attributes.GetNamedItem("InPoint").Value);
                MediaDuration = TimeSpan.Parse(node.Attributes.GetNamedItem("MediaDuration").Value);
            }
            else
            {
                EventInPoint = new TimeSpan(0);
                MediaDuration = EventDuration;
            }

            EventTransition = TransitionTypeFromString(node.Attributes.GetNamedItem("Transition").Value);
            if (EventTransition != vmTransitionType.cut)
                EventTransitionTime = int.Parse(node.Attributes.GetNamedItem("TransitionTime").Value);
            else
                EventTransitionTime = 0;

            if (EventType == vmEventType.photos)
            {
                try { EventLooping = bool.Parse(node.Attributes.GetNamedItem("Looping").Value); }
                catch { }
                SlideshowInterval = int.Parse(node.Attributes.GetNamedItem("SlideInterval").Value);
                SlideshowTransition = TransitionTypeFromString(node.Attributes.GetNamedItem("SlideTransition").Value);
                if (SlideshowTransition != vmTransitionType.cut)
                    SlideshowTransitionTime = int.Parse(node.Attributes.GetNamedItem("SlideTransitionTime").Value);
                else
                    SlideshowTransitionTime = 0;
            }
            Overlay = node.Attributes.GetNamedItem("Overlay").Value;
            EventMuted = bool.Parse(node.Attributes.GetNamedItem("EventMuted").Value);
        }
    }
}