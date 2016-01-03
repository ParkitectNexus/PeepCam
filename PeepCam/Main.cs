using UnityEngine;

namespace PeepCam
{
    public class Main : IMod
    {
        private static GameObject go;
        
        public void onEnabled()
        {
            go = new GameObject();

            go.AddComponent<PeepCam>();
        }

        public void onDisabled()
        {
            Object.Destroy(go);
        }

        public string Name { get { return "Peep Cam"; } }
        public string Description { get { return "Walk around in your own park!"; } }
        public string Identifier { get; set; }
    }
}
