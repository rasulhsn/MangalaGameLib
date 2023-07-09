
namespace MangalaGameLib.Game.ObjectModel
{
    public sealed class MangalaBoard<T>
    {
        public T[] Board { get; set; }
        public int Box { get; set; }
        public MangalaBoard()
        {
            this.Board = new T[6];
            this.Box = 0;
        }
    }
}
