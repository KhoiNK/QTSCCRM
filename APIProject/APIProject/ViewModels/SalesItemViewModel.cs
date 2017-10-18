using APIProject.Model.Models;

public class SalesItemViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public string Unit { get; set; }

    public SalesItemViewModel(SalesItem dto)
    {
        this.ID = dto.ID;
        this.Name = dto.Name;
        this.Price = dto.Price;
        this.Unit = dto.Unit;
    }
}