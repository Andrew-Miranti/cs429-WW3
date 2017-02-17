public class Province
{
    public Province()
    {
        this.City = null;
        this.Owner = null;
    }

    public Province(City city, Player owner)
    {
        this.City = city;
        this.Owner = owner;
    }

    /**
     * The city contained in this province
     * If the province doesn't have a city then null
     */
    public City City { get; set; }

    /**
     * The player that owns or occupies this province
     * If the province is neutral then null
     */
    public Player Owner { get; set; }
}