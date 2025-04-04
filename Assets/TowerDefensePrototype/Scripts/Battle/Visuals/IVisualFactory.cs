namespace CastlePrototype.Battle.Visuals
{
    public interface IVisualFactory
    {
        T Create<T>(string id) where T : VisualObject;
        void Release(VisualObject visualObject);
    }
}