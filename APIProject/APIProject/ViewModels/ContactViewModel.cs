﻿using APIProject.Model.Models;

public class ContactViewModel
{
    public int ID { get; set; }
    public string AvatarUrl { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    public ContactViewModel(Contact dto)
    {
        this.ID = dto.ID;
        this.Name = dto.Name;
        this.Position = dto.Position;
        this.Phone = dto.Phone;
        this.Email = dto.Email;
        this.AvatarUrl = dto.AvatarSrc;
    }
}