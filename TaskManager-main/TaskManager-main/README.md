# Manager de sarcini personale (Task Manager)

Aplicație desktop Windows pentru gestionarea unei liste de sarcini personale,
cu salvarea datelor între sesiuni într-un fișier JSON.

## Cerințe minime

- **Sistem de operare:** Windows 10 sau mai nou.
- **.NET SDK:** 8.0 sau mai nou ([descărcare](https://dotnet.microsoft.com/download)).
- **IDE recomandat:** Visual Studio 2022 (Community / Professional / Enterprise).

## Structura soluției

Soluția este împărțită în **trei proiecte (straturi)** conform principiilor
arhitecturii „Clean Architecture”:

```
TaskManager.sln
├── TaskManager.Models/             ← Stratul de date (POCO)
│   ├── TaskItem.cs                 ← Entitatea sarcină
│   ├── Priority.cs                 ← Enum prioritate (Low/Medium/High)
│   ├── Category.cs                 ← Enum categorie (Work/Study/Home/...)
│   └── TaskStatus.cs               ← Enum stare (New/InProgress/Completed)
│
├── TaskManager.BusinessLogic/      ← Stratul de business
│   ├── ITaskRepository.cs          ← Contract pentru depozit
│   ├── JsonTaskRepository.cs       ← Implementare JSON
│   ├── TaskService.cs              ← CRUD + validare
│   ├── TaskFilter.cs               ← Filtrare/sortare/căutare LINQ
│   ├── SortField.cs                ← Enum câmp sortare + direcție
│   ├── CsvExporter.cs              ← Export CSV (RFC 4180)
│   └── NotificationService.cs      ← Notificări termen ce expiră
│
└── TaskManager.UI/                 ← Stratul de prezentare (WinForms)
    ├── Program.cs                  ← Punctul de intrare
    ├── MainForm.cs / .Designer.cs  ← Formularul principal
    ├── TaskEditForm.cs / .Designer.cs ← Dialog adăugare/editare
    ├── ThemeManager.cs             ← Teme deschisă/întunecată
    └── app.manifest                ← Manifest DPI + Common Controls v6
```

## Funcționalități

### Funcționalități de bază

- **CRUD complet:** adăugare, modificare, ștergere sarcini.
- **Câmpuri:** titlu, descriere, dată creare, termen-limită, prioritate, categorie, stare.
- **Filtrare** după stare, categorie, prioritate.
- **Sortare** după dată creare, termen, prioritate sau titlu (ascendent/descendent).
- **Căutare** după titlu și descriere (case-insensitive).
- **Persistență JSON** în `%AppData%\TaskManager\tasks.json`.
- **Salvare automată** după fiecare modificare și la închiderea aplicației.

### Funcționalități suplimentare

- **Notificări** pentru sarcini cu termen ce expiră în următoarele 24 de ore
  și pentru sarcini deja restante (verificare la fiecare 5 minute).
- **Export CSV** (UTF-8 cu BOM, RFC 4180) — Excel afișează corect diacriticele.
- **Teme vizuale** — comutare între temă deschisă și temă întunecată
  prin meniul „Aspect”.

## Tehnologii folosite

| Tehnologie | Folosire |
|-----------|----------|
| .NET 8 | Framework de bază |
| C# 12 | Limbaj |
| WinForms | Interfață utilizator |
| `System.Text.Json` | Serializare JSON |
| LINQ | Filtrare, sortare, căutare |
| `System.Windows.Forms.Timer` | Verificare periodică notificări |
| `Guid` | Identificator unic sarcini |

## Pornire

### Din Visual Studio

1. Deschideți `TaskManager.sln`.
2. Setați `TaskManager.UI` ca proiect de pornire (click dreapta → Set as Startup Project).
3. Apăsați F5.

### Din linia de comandă (PowerShell / CMD)

```powershell
cd TaskManager
dotnet build
dotnet run --project TaskManager.UI
```

## Locația datelor

Sarcinile sunt salvate în:

```
%AppData%\TaskManager\tasks.json
```

Pe un cont „Ion” se traduce în:

```
C:\Users\Ion\AppData\Roaming\TaskManager\tasks.json
```

Fișierul are structura:

```json
[
  {
    "Id": "11111111-1111-1111-1111-111111111111",
    "Title": "Exemplu",
    "Description": "Descriere sintetică",
    "CreatedAt": "2026-04-27T18:30:00",
    "DueDate": "2026-05-01T00:00:00",
    "Priority": 2,
    "Category": 1,
    "Status": 0
  }
]
```

| Câmp | Tip | Valori posibile |
|------|------|-----------------|
| `Priority` | int | 0=Low, 1=Medium, 2=High |
| `Category` | int | 0=Work, 1=Study, 2=Home, 3=Personal, 4=Other |
| `Status` | int | 0=New, 1=InProgress, 2=Completed |

## Tratarea erorilor

- **Fișier JSON corupt** — la pornire se afișează mesaj de eroare; aplicația
  pornește cu listă goală.
- **Drepturi insuficiente** la salvare — mesaj de eroare; modificările rămân
  în memorie.
- **Salvare atomică** — strategia „write-then-rename” garantează că un crash
  în timpul salvării nu strică fișierul existent.

## Documentație în cod

Toate clasele și metodele publice au comentarii XML detaliate (în limba
română) cu:

- `<summary>` — descrierea scopului.
- `<param>` — descrierea fiecărui parametru.
- `<returns>` — descrierea valorii returnate.
- `<exception>` — excepțiile posibile și cauzele lor.
- `<remarks>` — note despre design, decizii și utilizare.

Documentația poate fi generată ca XML (la build) sau ca HTML (cu DocFX).
