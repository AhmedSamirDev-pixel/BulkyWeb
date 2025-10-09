# 🛒 BulkyBookWeb

## 📖 Overview
**BulkyBookWeb** is a full-featured eCommerce web application built using **ASP.NET Core MVC** and **Entity Framework Core**.  
It was developed as part of my .NET learning journey to apply real-world C# concepts, layered architecture, and database-driven development.

This project allows administrators to manage products, categories, and content, while customers can browse items and interact with a user-friendly interface.

---

## ⚙️ Features
- 🧩 **Product Management** – Create, update, and delete products dynamically.  
- 🗂️ **Category Management** – Organize products by category.  
- 🛍️ **Shopping Cart System** – Add, view, and update cart items.  
- 🔐 **Admin Dashboard** – Manage products, categories, and content easily.  
- 🧱 **Layered Architecture** – Separation between Data, Business, and Presentation layers.  
- 💾 **Entity Framework Core** – Code-first approach with SQL Server integration.  
- 🧠 **Repository Pattern** – Clean, reusable data access logic.  
- 🌐 **Modern UI** – Built with Bootstrap 5 and Razor pages.

---

## 🧠 Technologies Used
- ASP.NET Core MVC (C#)  
- Entity Framework Core  
- SQL Server  
- Bootstrap 5  
- HTML, CSS, JavaScript  
- Git & GitHub

---

## 🧩 Project Architecture
The project follows a **Clean Layered Architecture**:

```
BulkyBookWeb/
│
├── Bulky.DataAccess/ # Handles repositories and database context
├── Bulky.Models/ # Entity models
├── Bulky.Utility/ # Helper classes and constants
├── BulkyWeb/ # Main ASP.NET Core MVC project (UI layer)
└── README.md # Project documentation
```


---

## 🚀 Getting Started

### 1️⃣ Clone the Repository
```
git clone https://github.com/semido12/BulkyBookWeb.git
```

2️⃣ Open the Project

Open Bulky.sln in Visual Studio.

3️⃣ Configure the Database

1. Update your connection string in appsettings.json:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=BulkyDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

2. Run migrations:
```
update-database
```

4️⃣ Run the Application

Press F5 or click Run in Visual Studio to start the app

---

## 🧾 Documentation

This project demonstrates key ASP.NET Core MVC and Entity Framework concepts such as:

Controllers, Models, and Views

Repository Pattern & Dependency Injection

CRUD Operations

MVC Routing

Static File Management

Database Migrations

Image Uploads

📘 Detailed technical documentation will be added later in the /docs folder.

---

## Author

Ahmed Samir

🎓 Faculty of Artificial Intelligence Graduate

📧 ahmedsemido14@gmail.com

🌐 GitHub Profile

---

## Future Enhancements

Add order and payment modules

Implement Identity authentication

Add product reviews and ratings

Build RESTful API endpoints for integration



