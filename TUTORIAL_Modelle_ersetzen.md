# Charaktere & Objekte durch eigene Modelle ersetzen (z. B. Meshy3D GLB)

Das Spiel baut alle Figuren und Deko zur Laufzeit aus Code. Du musst **nichts am
Code ändern**, um eigene Modelle einzusetzen. Es gibt ein Drop-in-System:

> Lege ein Modell mit dem richtigen **Dateinamen** in den Ordner
> `Assets/Resources/Models/` – fertig. Fehlt ein Modell, nutzt das Spiel
> automatisch die bisherige Platzhalter-Optik weiter.

---

## 1. Namens-Konvention (wichtig!)

Der Dateiname (ohne Endung) muss **exakt** so heißen:

| Schlüssel        | Ersetzt …                                  |
|------------------|--------------------------------------------|
| `Character_0`    | Spieler 1 (Bloop) – Brett **und** Minispiel|
| `Character_1`    | Spieler 2 (Nomi)                           |
| `Character_2`    | Spieler 3 (Taro)                           |
| `Character_3`    | Spieler 4 (Pippa)                          |
| `Character`      | Fallback für **alle** Spieler              |
| `Prop_Tree`      | Baum                                       |
| `Prop_Rock`      | Fels / Stein                               |
| `Prop_Crystal`   | Kristall (dreht sich leicht)               |
| `Prop_Flag`      | Flagge                                     |
| `Prop_Lantern`   | Laterne (4 Ecken)                          |

Beispiel: `Assets/Resources/Models/Prop_Tree.glb` ersetzt alle Bäume.

Groß-/Kleinschreibung beachten. Keine Leerzeichen.

---

## 2. In Meshy3D exportieren

1. Modell generieren.
2. Export → **GLB** (oder FBX, siehe Schritt 3).
3. Empfehlungen für ein mobiles Party-Game:
   - **Low/Medium Poly** (Figuren grob < 10–20k Tris). Spart Akku.
   - Texturgröße 1024 oder 512 reicht für so kleine Figuren auf dem Bildschirm.
   - Y-up, wenn wählbar.
   - Wenn möglich „Single mesh / merged“, damit es 1 sauberes Objekt ist.

---

## 3. GLB in Unity importierbar machen

Unity 6 importiert **FBX** von Haus aus, **GLB nicht**. Du hast zwei Wege:

### Weg A – GLB direkt (empfohlen, weil du GLB nutzt)
Einmalig das offizielle glTFast-Paket installieren:

1. `Window → Package Manager`
2. Oben links `+` → **Add package by name…**
3. Name eingeben: `com.unity.cloud.gltfast` → **Add**

Danach importiert Unity `.glb`/`.gltf` automatisch als Modell-Asset.

### Weg B – ohne Paket (FBX statt GLB)
In Meshy einfach **FBX** exportieren. FBX importiert Unity ohne Zusatzpaket.
Sonst identisches Vorgehen.

---

## 4. Modell ins Projekt legen

1. Lege die Datei in `Assets/Resources/Models/` ab, z. B. `Character_0.glb`.
2. Unity importiert sie automatisch (kurz warten).
3. Im Editor `Play` drücken (Szene `Assets/Scenes/main.unity`).
   → Spieler 1 nutzt jetzt dein Modell, der Boden-Schatten und das
   Hüpfen/Squash funktionieren weiter automatisch.

> Reine Deko/Props (`Prop_*`) brauchen nichts weiter – Datei rein, fertig.

---

## 5. Charaktere einfärben (optional)

Du hast zwei Möglichkeiten:

**(a) Vier verschiedene Modelle:** `Character_0..3` – jede Figur sieht anders aus,
keine Einfärbung nötig.

**(b) Ein neutrales Modell, 4× umgefärbt** (blau/rot/grün/gelb):

1. Ziehe das importierte Modell einmal in eine Szene.
2. `GameObject` im Hierarchy-Fenster → rüberziehen in `Assets/Resources/Models/`
   → es entsteht ein **Prefab**. Benenne es `Character` (Fallback) oder
   `Character_0` usw.
3. Wähle das Prefab, im Inspector **Add Component → `ModelTint`**.
4. `Strength = 1`. (Optional bei „Targets“ nur bestimmte Renderer eintragen,
   z. B. nur den Körper, nicht die Augen.)
5. Lösche das Test-Objekt wieder aus der Szene.

`ModelTint` multipliziert die Basisfarbe → ein helles/neutrales Modell nimmt die
Spielerfarbe sauber an. Für Props lässt du `ModelTint` einfach weg.

> Tipp: Wenn du `ModelTint` brauchst, musst du den Weg über ein **Prefab** gehen
> (Schritt 2), weil man an die rohe importierte Datei keine Komponente hängen
> kann. Reine farbige Modelle ohne Tint funktionieren auch direkt als Datei.

---

## 6. Größe, Ausrichtung, Pivot

- **Höhe:** Spielfiguren ca. **0,9–1,3** Unity-Units hoch. Ist dein Modell zu
  groß/klein: importiertes Modell anklicken → Inspector → `Model` → `Scale Factor`
  anpassen (oder am Prefab die Scale setzen). Faustregel: lieber am Modell/Prefab
  skalieren, nicht im Code.
- **Pivot/Ursprung:** idealerweise **unten an den Füßen** (y = 0). Dann steht die
  Figur sauber auf dem Feld. Sitzt der Pivot in der Mitte, „schwebt“ oder
  „versinkt“ sie – dann in Meshy/Blender den Ursprung nach unten setzen oder im
  Prefab das Mesh-Child nach oben verschieben.
- **Blickrichtung:** Figuren schauen am besten Richtung **−Z** (zur Kamera).
  Falls sie falsch herum steht: am Prefab `Rotation Y = 180` setzen.

---

## 7. Materialien / „pinke“ Modelle reparieren

Dieses Projekt nutzt die **Built-in Render Pipeline** (kein URP).

- **FBX:** Texturen meist automatisch korrekt (Standard-Shader).
- **GLB via glTFast:** sollte direkt rendern. Falls etwas **pink/magenta** ist,
  stimmt der Shader nicht:
  1. Material im Projekt anklicken.
  2. Oben `Shader` → `Standard` wählen.
  3. Die Textur (Albedo/BaseColor) ins `Albedo`-Feld ziehen.
- Emission/Glow für Kristalle & Laternen macht das Spiel **nicht** automatisch für
  deine Modelle. Willst du Glühen: am Material `Emission` aktivieren und eine
  Emission-Farbe setzen.

---

## 8. Schnelltest-Checkliste

- [ ] Datei liegt in `Assets/Resources/Models/` mit exaktem Namen.
- [ ] Unity hat sie importiert (erscheint im Project-Fenster).
- [ ] `Play` auf `main.unity` → Figur/Prop erscheint.
- [ ] Größe passt (nicht riesig/winzig) – sonst Scale Factor.
- [ ] Steht aufrecht und richtig herum – sonst Rotation.
- [ ] Nicht pink – sonst Shader auf `Standard`.

---

## 9. Häufige Probleme

| Symptom                        | Ursache / Lösung                                  |
|--------------------------------|---------------------------------------------------|
| Modell erscheint nicht         | Name falsch, oder nicht in `Resources/Models/`    |
| Modell pink/magenta            | Shader → `Standard`, Textur in `Albedo`           |
| Riesig oder winzig             | `Scale Factor` im Model-Import / Prefab-Scale     |
| Liegt/steht falsch             | Rotation am Prefab (oft `Y = 180` oder `X = -90`) |
| Schwebt / versinkt             | Pivot nicht an den Füßen → Ursprung anpassen      |
| GLB lädt nicht trotz glTFast   | Modell in Szene ziehen → Prefab daraus bauen →    |
|                                | Prefab (richtiger Name) in `Resources/Models/`    |
| Spielerfarbe greift nicht      | `ModelTint`-Komponente am Prefab fehlt            |

---

## 10. Wieder zur Platzhalter-Optik zurück

Modell-Datei aus `Assets/Resources/Models/` löschen (oder umbenennen) → das Spiel
fällt automatisch auf die generierte Figur/Deko zurück. Nichts am Code nötig.
