using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchestra.Sandbox
{
    class Class1
    {
        public Class1()
        {
            Dispatch.VolumeChanged += this.VolumeChanged;
        }

        public void VolumeChanged(float volume)
        {
        }
    }
}
