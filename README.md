Project overview

root/
├─ CapstoneMaui/                  # .NET MAUI app (Android/Windows/iOS)
│  ├─ App.xaml, MainPage.razor    # UI & navigation
│  ├─ MauiProgram.cs              # DI container & app startup
│  └─ Platforms/                  # platform specifics
│
├─ Capstone.Core/                 # UI-agnostic logic 
│  ├─ Abstractions/               # public interfaces (IAuthService, ITimeEntryService, ...)
│  ├─ Clients/                    # API clients (HttpClient/Refit/fetch logic)
│  ├─ Services/                   # business services (compose Clients + domain rules)
│  ├─ Contracts/                  # DTOs / request/response models
│  ├─ Options/                    # strongly-typed config (ApiOptions, AuthOptions)
│  ├─ Extensions/                 # DI extension: AddCoreServices(...)
│  └─ Capstone.Core.csproj
│
└─ tests/
   └─ CapstoneMaui.Tests.NUnit/   # NUnit tests (mostly for Capstone.Core)
      ├─ Mocks/                   # test doubles or AutoFixture customizations
      ├─ Services/                # unit tests for Services
      ├─ Clients/                 # unit tests for API client logic
      └─ Test.csproj
