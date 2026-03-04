using FourZug.Backend.Adapter;

namespace FourZug.Backend.BackendManager
{
    public abstract class A_BackendManager
    {
        public A_Adapter? accessAdapter { get; set; }
    }
}