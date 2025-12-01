# Bulky 📚 – Full E-Commerce Web Application

Bulky is a complete e-commerce platform that simplifies online shopping and product management. It includes everything from product and category management to authentication, shopping carts, orders, and payment processing. Built with ASP.NET Core MVC, Entity Framework Core, and the Repository + Unit of Work patterns, Bulky provides a scalable and maintainable architecture suitable for real-world business needs.

---

## 🚀 Key Features

###  **🛍️ Product Management**:

* Create, edit, and manage product information including title, description, ISBN, author, pricing tiers, and product images.



###  **📂 Category & Company Management**:

* Manage product categories and company information to organize your catalog.



###  **👤 Authentication & Authorization**:

* Using ASP.NET Core Identity, supporting roles:

  * Customer

  * Employee

  * Admin

  * Company

* Includes login, registration, and external authentication.



###  **🛒 Shopping Cart**:

* Customers can add items, modify quantities, and proceed to checkout.



###  **📦 Order & Checkout System**:

* Complete order lifecycle:

  * Create orders

  * Track order status

  * View order history

  * Admin order management



###  **💳 Stripe Payment Integration**:

* Secure online payments using Stripe Checkout:

  * Supports card payments

  * Order status automatically updates based on payment outcome



###  **⚙️ Database Initialization**:

* Auto-creates:

  * Default roles

  * Admin account

  * Seed categories and companies (optional)



###  **✉️ Email Notifications**:

* Email sender service for identity and order updates (configurable with SMTP).



###  **📁 Clean Architecture**:

* Built using:

  * Repository Pattern

  * Unit of Work Pattern

  * Dependency Injection

  * Separation of Concerns across layers



###  **🌐 Azure Deployment**:

* Application is fully deployed on Azure App Service with:

  * Azure SQL Database

  * Production appsettings.json

  * HTTPS enforced

 

###  **Live Demo**:
👉 https://bulkywebapp-bua8a8daffc0azff.westeurope-01.azurewebsites.net/


---


💻 **Project Structure**

```
Bulky/
├── Bulky.sln                                  # Solution file
│
├── Bulky.DataAccess/                          # Data Access Layer (DAL)
│   ├── Data/                                   # Database Context
│   │   └── ApplicationDbContext.cs
│   │
│   ├── DbInitializer/                          # Database Seeding
│   │   ├── IDbInitializer.cs
│   │   └── DbInitializer.cs
│   │
│   ├── Repository/                             # Repository Pattern
│   │   ├── IRepository/                        # Interfaces
│   │   │   ├── IRepository.cs                  # Generic repo interface
│   │   │   ├── IUnitOfWork.cs                 # Unit of Work interface
│   │   │   ├── ICategoryRepository.cs
│   │   │   ├── ICompanyRepository.cs
│   │   │   ├── IProductRepository.cs
│   │   │   ├── IShoppingCartRepository.cs
│   │   │
│   │   ├── Repository.cs                      # Generic repository implementation
│   │   ├── UnitOfWork.cs                      # Unit of Work implementation
│   │   ├── CategoryRepository.cs
│   │   ├── CompanyRepository.cs
│   │   ├── ProductRepository.cs
│   │   └── ShoppingCartRepository.cs
│
├── Bulky.Models/                              # Domain Models (Entities)
│   ├── ApplicationUser.cs
│   ├── Category.cs
│   ├── Company.cs
│   ├── Product.cs
│   ├── ShoppingCart.cs
│   ├── OrderHeader.cs
│   └── OrderDetail.cs
│
├── Bulky.Utility/                             # Shared utilities & helpers
│   ├── SD.cs                                  # Static constants
│   └── EmailSender.cs                         # Email sender service
│
├── BulkyWeb/                                  # ASP.NET Core MVC Web App
│   ├── Areas/                                 # Admin & Customer areas
│   │   ├── Admin/
│   │   └── Customer/
│   │
│   ├── Controllers/                           # MVC Controllers
│   ├── Models/                                # ViewModels (DTOs)
│   ├── Views/                                 # MVC Views
│   ├── Pages/                                 # Razor Pages (Identity)
│   │
│   ├── Data/                                  # Additional Web layer data configs
│   ├── wwwroot/                               # Static files (CSS, JS, images)
│   ├── appsettings.json                       # Configuration
│   └── Program.cs                             # Entry point


```


---


## 🛠️ Tech Stack

| Category           | Technology                                             |
| ------------------ | ------------------------------------------------------ |
| **Frontend**       | ASP.NET Core MVC, Razor Pages, HTML, CSS, JavaScript   |
| **Backend**        | C#, ASP.NET Core 7                                     |
| **Database**       | SQL Server, Entity Framework Core                      |
| **Authentication** | ASP.NET Core Identity                                  |
| **Payment**        | Stripe                                                 |
| **Email**          | IEmailSender (Identity UI)                             |
| **Architecture**   | Repository Pattern, Unit of Work, Dependency Injection |
| **Environment**    | Azure App Service, Azure SQL                           |
| **Tools**          | Visual Studio 2022                                     |

---


## 📦 Getting Started


###  **Prerequisites**:

* Visual Studio 2022

* .NET 7 SDK

* SQL Server

* Stripe account (for payments)

---

## 🔧 Installation


###  **1️⃣ Clone the Repository**:

* Open a terminal (CMD, PowerShell, or Git Bash) and run:

```
git clone <repository_url>
```

* This downloads the entire project to your machine.


###  **2️⃣ Open the Solution**:

* Open Visual Studio 2022 (or later).

* Click File → Open → Project/Solution.

* Navigate to the cloned folder.

* Select:

```
Bulky/Bulky.sln
```

* This loads all projects:
  
 * ✔ BulkyWeb
 * ✔ Bulky.DataAccess
 * ✔ Bulky.Models
 * ✔ Bulky.Utility


###  **3️⃣ Configure the Database Connection**:


You must point the project to your SQL Server.

1. Open the file:


```
BulkyWeb/appsettings.json
```



2. Find the ConnectionStrings section and update it with your SQL Server name:


```
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=Bulky;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```



If you use SQL Server Authentication (username + password):

```
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=Bulky;User Id=sa;Password=YourPassword;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```



###  **4️⃣ Apply Database Migrations**:

* This step creates all tables, roles, and relationships.

* Open Package Manager Console in Visual Studio:
  
  * View → Other Windows → Package Manager Console

* Run:


```
Update-Database
```


*  This will:

  * ✔ Create the database
  * ✔ Create tables (Products, Categories, Users, Orders, ShoppingCart, etc.)
  * ✔ Seed default roles
  * ✔ Create the admin user (via DbInitializer)




###  **5️⃣ Configure Stripe Payment**:

* Step 1 — Add Stripe keys to appsettings.json

* Open:


```
BulkyWeb/appsettings.json
```


* Add or update this section:


```
"Stripe": {
  "SecretKey": "your_secret_key",
  "PublishableKey": "your_publishable_key"
}
```


👉 You get these keys from your Stripe Dashboard → Developers → API Keys.


* Step 2 — Add Stripe to Program.cs

* Open:


```
BulkyWeb/Program.cs
```


* Add:


```
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
```


### This sets Stripe up globally so payment sessions can be created.



###  **6️⃣ Run the Application**:


* In Visual Studio, set BulkyWeb as the startup project.

** Build the solution:
** Build → Build Solution

* Run the app:
** Press F5 or click the green Run button.

The app will open at a URL such as:

```
https://localhost:7020
```

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and commit them with descriptive messages.
4.  Push your changes to your fork.
5.  Submit a pull request.

---


## 📬 Contact

If you have any questions or suggestions, please feel free to contact me at email: ahmedsamir.dev.30@gmail.com.


---


## 💖 Thanks

Thank you for checking out Bulky! We hope you find it useful.
