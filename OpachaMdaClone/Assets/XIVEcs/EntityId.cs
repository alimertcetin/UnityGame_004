namespace XIV.Ecs
{
    public struct EntityId
    {
        public int id;
        public int generation;

        public EntityId(int id, int generation)
        {
            this.id = id;
            this.generation = generation;
        }
    }
}