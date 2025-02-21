using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.EntryPoint
{
    public interface ICompose 
    {
        public void Compose(DiContainer diContainer);
    }
}