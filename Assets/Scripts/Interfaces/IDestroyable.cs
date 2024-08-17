using System;
using UnityEngine.Events;

namespace Interfaces
{
    public interface IDestroyable
    {
        public event EventHandler DestructionEvent;

     
    }
}