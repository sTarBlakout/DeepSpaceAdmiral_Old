using System.Collections.Generic;

namespace RTS.Interfaces
{
    public interface ICarriable
    {
        void LaunchSquadron(int id);
        List<int> SquadronIds { get; }
    }
}