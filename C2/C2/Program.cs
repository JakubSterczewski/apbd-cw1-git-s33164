// See https://aka.ms/new-console-template for more information

using C2;

Console.WriteLine("Hello, World!");

RentalService rentalService = new RentalService();
Student student = new Student("John", "Doe");
Employee employee = new Employee("John", "Doe");
Laptop laptop = new Laptop("Asus", "JakasNazwa", 12, "Procer nazwa");
Laptop laptop2 = new Laptop("Asus", "JakasNazwa", 12, "Procer nazwa2");
Laptop laptop3 = new Laptop("Asus", "JakasNazwa", 12, "Procer nazwa3");

rentalService.RentEquipment(student, laptop);
rentalService.RentEquipment(student, laptop2);
rentalService.ReturnEquipment(laptop2);
rentalService.RentEquipment(student, laptop2);
rentalService.RentEquipment(student, laptop3);
