using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interfaces
{
    public interface IDestroyable
    {
        public delegate void DestroyableDelegate(GameObject sender, EventArgs e);
        public event DestroyableDelegate DestructionEvent;

     
    }
}