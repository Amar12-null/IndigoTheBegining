# Indigo The Beginning — NPC Customer System Rundown

Ovaj dokument opisuje sve dodano za NPC customer system, day progression, money/XP, shop i integraciju sa coffee making sistemom.

---

## 1. NOVI FAJLOVI

### `Assets/Scripts/Audio/` (novi folder)
| Fajl | Opis |
|---|---|
| `SoundManager.cs` | Singleton sa AudioSource pool-om (8 source-a), music source, master/sfx/music volume. 15 SfxId enuma. |
| `SoundBindings.cs` | Sluša game evente i pušta SFX: tip, angry leave, level up, purchase, day start/end. |

### `Assets/Scripts/Game/` (novi folder)
| Fajl | Opis |
|---|---|
| `DayManager.cs` | Kontroliše dane. Phases: `PreDay → Active → Ended`. Skaliranje težine po danu (spawn rate raste, patience opada). |
| `DayHUD.cs` | Panel gore-sredina: "Day N" + timer + countdown bar (zeleno → žuto → crveno). |
| `EndOfDayPanel.cs` | Pauza panel: Day, Rating (S/A/B/C/D), Served, Lost, Earned + "Start Next Day" dugme. |
| `GameStateManager.cs` | Formalni state machine: Boot, DayActive, Paused, DayEnded, Shopping. |

### `Assets/Scripts/Integration/` (novi folder)
| Fajl | Opis |
|---|---|
| `CupOrderBridge.cs` | Bridge između Amarovog `CupPickup` i NPC sistema. Polling-based, ne mijenja Amarov kod. |
| `DrinkSelectorStation.cs` | Pored CoffeeMachineSlot-a. Tipke 1-4 biraju piće, dodjeljuje drinkType brewovanim šoljama. |

### `Assets/Scripts/Progression/` (novi folder)
| Fajl | Opis |
|---|---|
| `PlayerProgression.cs` | Static XP/level sistem. XP po satisfaction tier-u (Happy=12, Neutral=7, Annoyed=3). Level threshold = 40 + (lvl-1)*30. Sadrži i `XpBridge` MonoBehaviour. |
| `ProgressionHUD.cs` | Panel desno: "Lv X" + XP bar + "current/required XP". |

### `Assets/Scripts/Shop/` (novi folder)
| Fajl | Opis |
|---|---|
| `Upgrades.cs` | Catalog 4 upgrade-a, static API za purchase + getteri za efekte. |
| `ShopPanel.cs` | Full-screen shop panel, otvara/zatvara sa **B**. Buy button-i sa "OWNED"/"Lv X"/"$Y" stanjima. |

### `Assets/Scripts/Customers/` (postojeći folder — dodato)
| Fajl | Opis |
|---|---|
| `MoneyManager.cs` | Static singleton. Balance, TotalEarned, CustomersServed, CustomersLost. Eventi za HUD/popup. |
| `MoneyHUD.cs` | Panel gore-desno: "$X" + "Served N · Lost M" + punch scale animacija na svaki tip. |
| `MoneyPopup.cs` | Float-up "+$5" tekst iznad gosta; "ANGRY!" kad ode bijesan. Sadrži i `MoneyPopupSpawner`. |

### `Assets/Scripts/UI/` (postojeći folder — dodato)
| Fajl | Opis |
|---|---|
| `HelpHUD.cs` | Mali "E Interact · Q Drop · B Shop · ESC Pause" hint dolje u sredini. |

---

## 2. MODIFIKOVANI FAJLOVI

| Fajl | Šta je dodato/promijenjeno |
|---|---|
| `Customers/Customer.cs` | Patience timers, satisfaction enum, money payout, XP integracija, random visual variants, Entering state fix, pause check, defensive empty-array check. |
| `Customers/CustomerInteraction.cs` | Destroy cup on successful delivery (kroz CupOrderBridge gameObject). |
| `Customers/CustomerSpawner.cs` | Pause check, DayManager check (spawn samo dok je day Active), upgrades concurrent customer count. |
| `Customers/OrderBubble.cs` | Patience bar koji se prazni, mood color (zelena/plava/žuta/crvena prema satisfaction-u). |
| `Customers/OrderType.cs` | Dodao `BasePrice()`, `Satisfaction` enum, `TipMultiplier()`, `Color()` extensions. |

---

## 3. UNITY EDITOR SETUP

### 3.1 Project-level (jednom)
- **NavMesh Surface** na podu kafića → Bake (AI Navigation paket potreban)
- **Tag "Player"** na Player GameObject-u
- **HandPoint** child Player-a na poziciji ruke (Amar koristi za držanje cup-a)

### 3.2 Obriši/ignoriši
- **MainMenu.cs** komponentu sa MainMenu GameObject-a u gameplay sceni (inače gradi title menu preko)

### 3.3 Novi GameObject-i u sceni

| Naziv | Komponenta(e) | Polja |
|---|---|---|
| `TableManager` | `Table Manager` | — |
| `DayManager` | `Day Manager` | Spawner = CustomerSpawner |
| `CustomerSpawner` | `Customer Spawner` | Customer Prefab, Spawn Point, Exit Point, intervali |
| `SoundManager` | `Sound Manager` | SFX array + ambient music clip |
| `GameStateManager` | `Game State Manager` | — |
| `SpawnPoint` | empty | Pozicija "vrata" na NavMesh-u, Z osa unutra |
| `ExitPoint` | empty | Pozicija izlaza na NavMesh-u |

### 3.4 Komponente na **GameUI** GameObject (Add Component za sve)
- `Money HUD`
- `Money Popup Spawner`
- `Order Tickets HUD`
- `Day HUD`
- `End Of Day Panel`
- `Progression HUD`
- `Shop Panel`
- `Xp Bridge` (iz Progression namespace-a)
- `Sound Bindings`
- `Help HUD`

### 3.5 Stolovi (3-4 komada)

Za svaki stol:
1. Drag `Cafe_Table_1.prefab` u scenu (`Assets/Mnostva Art/FREE_Interiors_2/Prefabs/`)
2. Add Component → `Table`
3. Dodaj child empty `SeatAnchor`, postavi na poziciju stolice, Z osa gleda prema stolu
4. (Opcionalno) child empty `BubbleAnchor` 1.8m iznad
5. Drag `Cafe_Chair_1.prefab` kao child, pozicioniraj
6. Drag SeatAnchor u Table komponentu (polje Seat Anchor)

### 3.6 Customer prefab

1. Empty GameObject
2. Add Component → `NavMesh Agent`, `Customer`, `Customer Interaction`, **Capsule Collider** (Is Trigger ✓, radius 1)
3. NavMeshAgent postavke: Radius=0.3, Height=1.5, Speed=1.5
4. Customer komponenta:
   - **Visual Variants** array: drag svih 9 `stickman_1` do `stickman_9` iz `Assets/Hyper Casual Characters/Prefab/`
   - **Possible Orders** array: ostavi default (sva 4 pića)
   - Patience parametri po želji
5. Save kao `Customer.prefab` u `Assets/Prefabs/`

### 3.7 Cup prefab (Amarov + moj bridge)

1. Otvori Amarov `Cup` prefab
2. Add Component → `Cup Order Bridge`
3. Default Drink Type = Espresso (drink selector će ga prepisati pri brewing-u)
4. Sačuvaj prefab

### 3.8 DrinkSelectorStation

1. Empty GameObject pored Coffee Machine-a, npr. `DrinkSelector`
2. Add Component → **Box Collider** (Is Trigger ✓, oko mašine)
3. Add Component → `Drink Selector Station`
4. Drag `CoffeeMachineSlot` GameObject u **Machine** polje

### 3.9 SoundManager — popuni clip-ove

Za svaki SfxId enum dragni AudioClip:
- `Pickup`, `Drop`, `Place`, `BrewStart`, `BrewLoop`, `BrewDone`
- `DeliverySuccess`, `TipCoin`, `LevelUp`, `Purchase`
- `DayStart`, `DayEnd`, `CustomerAngry`, `DoorOpen`, `ButtonClick`

Besplatni izvori: [freesound.org](https://freesound.org), [kenney.nl/assets/interface-sounds](https://kenney.nl/assets/interface-sounds)

---

## 4. GAMEPLAY FLOW

1. Pritisni **Play**
2. Day 1 počinje za ~3s, NPC počnu da dolaze
3. Pristupi NPC-u → **E** da uzmeš narudžbu
4. Idi do mašine: pokupi portafilter (Amarova logika), mljej (E), na mašinu, postavi cup
5. Pritisni **1-4** kod mašine da izabereš tip pića
6. Hold left click — brewing
7. Pokupi šolju, donesi gostu → **E** dostavi
8. **+$X** popup, XP raste, gost ode
9. Ponavljaj. **B** za shop ako imaš novac
10. Kad timer padne na 0 → end-of-day rezime → "Start Next Day"
11. Sljedeći dan: brži spawn, manje strpljenja

---

## 5. CONTROLS

| Tipka | Akcija |
|---|---|
| **WASD / arrows** | Kretanje |
| **Shift** | Trčanje |
| **Space** | Skok |
| **R** | Čučanj |
| **E** | Interakcija (pickup, take order, deliver, place) |
| **Q** | Drop cup (ako je u CupPickup integraciji) |
| **1-4** | Drink select kod mašine (Espresso/Cappuccino/Latte/Americano) |
| **B** | Otvori/zatvori Shop |
| **ESC** | Pauza |

---

## 6. UPGRADES (Shop)

| Upgrade | Cijena | Level | Efekat |
|---|---|---|---|
| Tip Jar | $25 | 1 | +25% napojnice |
| Comfy Chairs | $50 | 2 | +10s strpljenja za nove goste |
| Reputation Sign | $75 | 3 | Gosti su sretni duže (lakše Happy tier) |
| Marketing Posters | $120 | 5 | +1 maks istovremenih gostiju |

---

## 7. DRINK PRICES + TIP MULTIPLIERS

**Base price po piću:**
- Espresso $3
- Americano $4
- Cappuccino $5
- Latte $6

**Satisfaction → tip multiplier:**
- Happy (>65% strpljenja): 1.6×
- Neutral (>30%): 1.0×
- Annoyed (>0%): 0.5×
- Angry (0%): 0×

**Sa Tip Jar upgrade-om: dodatnih ×1.25.**

---

## 8. XP TABLE

| Action | XP |
|---|---|
| Happy delivery | 12 |
| Neutral delivery | 7 |
| Annoyed delivery | 3 |
| Angry leave | 0 |

**Level threshold:** Level N treba `40 + (N-1) * 30` XP.

---

## 9. POZNATI PROBLEMI / NAPOMENE

- **Nisam pokrenuo Unity Play** — samo static code analysis. Mogući issue-i pri prvom run-u.
- **SoundManager treba clip-ove** — sve je tiho dok se ne assignuju AudioClip fajlovi.
- **Bez NavMesh Surface** + Bake-a NPC neće hodati.
- **Bez Player tag-a** NPC neće detektovati igrača za interakciju.
- **Cup mora imati `Cup Order Bridge`** — bez toga delivery ne radi.
- **Stara `MainMenu.cs` komponenta** u gameplay sceni će graditi title menu preko HUD-a.

---

## 10. ARHITEKTURNE NAPOMENE

- Sav novi kod je u **namespace-ovima** (`Indigo.Customers`, `Indigo.Game`, `Indigo.Shop`, `Indigo.Audio`, `Indigo.Progression`, `Indigo.Integration`, `Indigo.UI`).
- Amarov coffee making kod ostaje **bez namespace-a** (global) — ne mijenjao ga niti jednom.
- Komunikacija između sistema ide preko **static event bus-a**: `UIEvents`, `OrderRegistry`, `MoneyManager`, `PlayerProgression`, `Upgrades`.
- Customer state machine: `Entering → Seeking → Sitting → WaitingForOrderTaken → WaitingForDrink → Drinking → Leaving → Done`.

---

## 11. KAKO KONVERTOVATI U PDF

Ovaj fajl je markdown. Načini konverzije u PDF:

1. **VS Code**: instaliraj ekstenziju "Markdown PDF" → otvori RUNDOWN.md → desni klik → "Markdown PDF: Export (pdf)"
2. **Browser**: otvori markdown preview na GitHub-u (ako pushuješ) → Ctrl+P → Save as PDF
3. **Online**: [md-to-pdf.fly.dev](https://md-to-pdf.fly.dev/) ili [markdowntopdf.com](https://www.markdowntopdf.com/)
4. **Pandoc** (terminal): `pandoc RUNDOWN.md -o RUNDOWN.pdf`
