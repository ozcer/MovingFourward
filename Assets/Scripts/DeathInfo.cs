public class DeathInfo
{
    public Grid location;
    public GridObject killer;
    public GridObject victim;

    public DeathInfo(Grid deathLocation, GridObject killer, GridObject victim)
    {
        this.location = deathLocation;
        this.killer = killer;
        this.victim = victim;
    }
}