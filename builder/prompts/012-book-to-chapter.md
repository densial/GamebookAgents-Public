# 012 — BOOK TO CHAPTERS

## Inputs

The user will provide:

- **Book**

## Fixed chapter architecture

Each chapter contains exactly **68 numbered entries**.

This total is fixed by the five-sequence topology structure used later in the pipeline:

| Sequence | Entries |
|---|---:|
| Sequence 1 | 12 |
| Sequence 2 | 14 |
| Sequence 3 | 16 |
| Sequence 4 | 14 |
| Sequence 5 | 12 |
| **Total** | **68** |

Do not ask the user for a configurable chapter-entry count and do not vary this total at this stage.

Prompt 0122 plans the chapter-level encounter budget against these **68 entries**. Later sequence planning must preserve the fixed sequence sizes above, so the five sequences together always implement the same 68-entry chapter budget.

## Workspace layout and path resolution

This reusable prompt lives under `[Workspace]/builder/prompts/`. The shared builder root is `[Workspace]/builder/`, and each book is an independent sibling directory. `[Book]` means the existing book root supplied by the user, either as an absolute path or a path relative to `[Workspace]`.

Keep reusable prompts, topology files, and rules sources under the builder. Write book-specific planning and output files only under `[Book]`.

Use the Book value to locate the book directory. `[Book]` means the existing root folder for that book; do not create a second nested folder with the same name.

Read:

```text
[Book]/BOOK_CONCEPT.md
```

Also read:

```text
[Book]/FACTS.md
[Book]/STORY_TAGS.md
```

if they already exist.

`FACTS.md` stores canonical story truth.

`STORY_TAGS.md` stores rare, persistent player-facing gameplay state that may alter later **survival risk** or **material outcome**. It is not a fact registry, XP system, or record of every check.

Before planning, identify the book's **starting character level** and confirm that its progression is compatible with the milestone rule below. If the starting level is missing, or the concept explicitly requires incompatible advancement, stop and report what needs clarification or updating.

**Do not invent a starting level, XP curve, or additional level-ups.**

---

# 1. Book folder structure

Use this directory convention for the book's production files.

Chapter and sequence folders are **internal production organisation**, not visible divisions in the published gamebook.

```text
[Book]/
├── BOOK_CONCEPT.md
├── FACTS.md
├── STORY_TAGS.md
├── CHAPTER_OVERVIEWS.md
├── chapters/
│   ├── chapter-01/
│   │   ├── sequences.md                 (created by a later stage)
│   │   └── sequence-01/                 (created by a later stage)
│   │       ├── plan.md
│   │       ├── mechanics.md
│   │       ├── final.md
│   │       └── prose-validation.md
│   ├── chapter-02/
│   └── ... chapter-06/
└── output/                              (assembled publication files, later)
```

There is **no achievement-tag registry or XP ledger** in this workflow.

The workflow does use `[Book]/STORY_TAGS.md` for a **small number of persistent gameplay tags**. These tags are not achievements and do not award XP. They exist only when an earlier outcome needs to affect later **survival risk** or **material outcome** in a way not already represented by the character sheet.

An existing legacy `ACHIEVEMENT_TAGS.md` may remain on disk during migration, but do not read, create, update, or depend on it.

For **this stage only**:

- create `[Book]/chapters/chapter-01/` through `[Book]/chapters/chapter-06/` if the filesystem supports directory creation;
- create or update `[Book]/CHAPTER_OVERVIEWS.md`;
- create or update the Prompt-0122-owned content of `[Book]/FACTS.md`;
- create `[Book]/STORY_TAGS.md` if it does not exist, using the registry rules in this prompt;
- preserve any existing valid persistent story-tag records without silently changing their meaning.

Do not create:

- `sequences.md`;
- sequence folders;
- individual chapter publication files;
- mechanics;
- prose;
- anything under `[Book]/output/`.

Do not move or overwrite unrelated existing book files.

Folder names use two-digit chapter and sequence numbers.

Player-facing entry numbers are ordinary integers without leading zeroes and run consecutively across the book. Detailed entry numbering belongs to later planning stages.

---

# 2. Canon registry — `FACTS.md`

`[Book]/FACTS.md` is the book's **production-only canonical narrative memory**.

It exists so later stages do not need to reconstruct story truth from prose, guesses, or scattered upstream files.

It is not:

- player-facing text;
- a plot summary for publication;
- a mechanics file;
- a topology file;
- an encounter-budget ledger;
- an XP or achievement system;
- a place for speculative ideas.

The registry must distinguish:

1. what is objectively true in the story;
2. what the player knows and when;
3. what remains deliberately unresolved and must not be assumed.

## 2A. Prompt 012's authority

Prompt 012 is an **authorised high-level story-design stage**.

It may establish new **book-level or chapter-level canon** when necessary to turn `BOOK_CONCEPT.md` into one coherent six-chapter adventure.

However, it must never introduce canon silently.

Any new canonical fact created at this stage must:

- be compatible with `BOOK_CONCEPT.md`;
- be necessary or materially useful to the chapter-level adventure structure;
- appear explicitly in `CHAPTER_OVERVIEWS.md`;
- be recorded explicitly in `FACTS.md`;
- have a clear provenance;
- have a player-knowledge gate;
- remain at book/chapter scale rather than inventing entry-level evidence or prose detail.

Prompt 012 may decide high-level facts such as:

- a chapter's actual objective;
- the existence and narrative role of a major location;
- the identity or role of a major NPC if the book concept leaves this to chapter planning;
- a major cause, threat, relationship, reveal, reversal, or resolution needed for the overall adventure;
- what common fact the player will learn by the end of a chapter.

Prompt 012 must **not** decide entry-level implementation details such as:

- exactly which footprint, scratch, guide mark, stain, letter, object, sound, or visual cue proves a fact;
- exact dialogue;
- incidental NPC actions;
- the precise appearance of evidence;
- exact branch-specific clues;
- exact combat circumstances;
- exact item placement;
- exact mechanical resolution;
- prose-level sensory detail.

Those belong to later authorised stages.

If a chapter-level development requires a major story decision that cannot be made without contradicting or exceeding the authority of `BOOK_CONCEPT.md`, stop and report the missing upstream decision instead of hiding the invention inside an overview.

---

# 3. No silent canonical invention

Treat the following as **canonical story facts** when they could affect later reasoning, continuity, mystery, choices, or player conclusions:

- identities;
- relationships;
- motives;
- objectives;
- histories;
- causes;
- locations;
- destinations;
- events;
- NPC actions;
- ownership;
- object significance;
- clue meanings;
- evidence;
- promises;
- betrayals;
- allegiances;
- world rules;
- supernatural causes;
- facts about what happened before play;
- facts the player can later rely on;
- conclusions that later entries are expected to treat as true.

Do not create any such fact merely because it makes a chapter easier to outline.

If Prompt 012 creates one under its authorised high-level story-design role, it must be **deliberate, explicit, and registered**.

A fact that is not in:

- `BOOK_CONCEPT.md`;
- the generated `CHAPTER_OVERVIEWS.md`;
- or `FACTS.md`;

must not be treated as established canon by this stage.

Do not rely on model memory, genre convention, implied setting lore, or likely explanations as substitutes for canon.

---

# 4. `FACTS.md` format

If `[Book]/FACTS.md` does not exist, create it with this structure:

```markdown
# FACTS

This file is production-only canonical narrative memory.
It is not player-facing text.

## Registry Rules

- A canonical fact must have an explicit source.
- Facts are never inferred merely from genre convention or likely explanation.
- Player knowledge is separate from objective truth.
- A fact marked as hidden or planned for a later reveal must not be assumed by earlier player-facing material.
- Optional route-specific knowledge must never become common knowledge without an explicit common reveal.
- Later stages may refine the exact point where a fact becomes known, but must not silently change the fact itself.
- Unresolved items must not be treated as facts until an authorised planning/verification stage resolves them.

## Canonical Facts

[Fact records]

## Not Yet Established / Must Not Be Assumed

[Only important unresolved matters that later stages might otherwise be tempted to assume.]
```

Each canonical fact record must use:

```markdown
### FACT-[NNNN]

**Fact:** [one concise canonical statement]  
**Category:** [Character / Relationship / Location / Event / Objective / Cause / World / Item / Other]  
**Source:** [exact source file and relevant section/chapter]  
**Player knowledge:** [Known from start / Hidden / Planned common reveal: Chapter X / Planned optional reveal: Chapter X / Not player-facing]  
**Established at:** [Start / Book concept / Planned Chapter X handoff / Not yet established in player-facing entries]
```

Use four-digit, monotonically increasing IDs:

```text
FACT-0001
FACT-0002
FACT-0003
```

Never renumber existing facts merely because the prompt is rerun.

Do not use fact IDs as player-facing tags or game state.

Fact IDs are production metadata only.

---

# 5. Existing `FACTS.md` safety

If `FACTS.md` already exists:

1. read it before planning;
2. preserve all existing fact IDs;
3. do not delete or silently rewrite facts owned by later stages;
4. do not change an existing fact merely because a new chapter outline would be easier if it were different;
5. identify any contradiction between existing canon and the new chapter plan before saving.

Prompt 012 may:

- add facts directly supported by `BOOK_CONCEPT.md`;
- add new high-level book/chapter canon deliberately established by the regenerated `CHAPTER_OVERVIEWS.md`;
- update a Prompt-012-created fact's chapter-level reveal timing when the chapter overview itself is explicitly being revised.

Prompt 012 must not silently revise downstream facts established by later sequence planning, verification, mechanics, or prose stages.

If downstream production files already exist and the requested Prompt 012 revision would invalidate them, stop and identify the affected downstream material rather than pretending the change is isolated.

---

# 6. Persistent gameplay tags — `STORY_TAGS.md`

`[Book]/STORY_TAGS.md` is the production registry for **rare persistent player-facing gameplay tags**.

These tags exist to let an earlier success, failure, or meaningful choice affect later play after the immediate branch has reconverged.

They are not:

- XP;
- achievements;
- canonical facts;
- a record of every check;
- a substitute for HP, money, equipment, spell slots, consumables, or conditions;
- mandatory keys for core story progression.

A persistent story tag is justified only when it later changes one or both of the book's two primary gameplay-value axes:

1. **Survival** — the player's chance of reaching the end alive.
2. **Material outcome** — the money, useful items, consumables, equipment, or other material value the player gains, preserves, spends, or loses.

Examples of appropriate tag effects include:

- a trusted contact later provides a safer route;
- a local-favour tag provides a discount;
- a guide tag allows a dangerous hazard to be bypassed;
- an access tag opens an optional loot opportunity;
- a negative reputation tag makes a later route more dangerous or expensive.

Do not create a persistent tag merely to remember:

- that a check succeeded or failed;
- that the player spoke to someone;
- that the player learned required information;
- that a route was taken;
- that an ordinary character-sheet resource changed.

## 6A. Core-progression rule

Persistent story tags may improve, worsen, or alter later play, but **core story information and mandatory progression must not depend on possessing an optional tag**.

The main adventure must remain completable without any particular optional persistent story tag.

A tag may change:

- risk;
- cost;
- access to an optional route;
- access to optional material reward;
- later mechanical difficulty;
- whether an avoidable encounter occurs.

It must not be the only way to obtain story-critical information required to continue.

## 6B. Tag economy

Keep persistent story tags sparse.

As a soft guideline for a six-chapter book:

- many sequences should create no persistent tag;
- a chapter will usually introduce **0–1** new persistent tags;
- occasionally a chapter may introduce **2** if both have clear later uses;
- prefer reusing an existing tag over creating another near-duplicate.

Prefer **positive tags with absence as the default state**.

For example:

```text
GREYFEN_TRUST
```

is usually better than maintaining both:

```text
GREYFEN_TRUST
GREYFEN_DISTRUST
```

Create a negative persistent tag only when the negative state itself has a meaningful later survival or material consequence.

## 6C. Prompt 0122 authority over tags

Prompt 0122 plans only **high-level persistent-tag opportunities**.

It must not invent exact tag identifiers, exact mechanical bonuses, exact prices, exact route numbers, or exact acquisition checks.

The chapter overview may say, for example:

```text
Persistent tag opportunity:
Local trust may later provide safer assistance or a material discount.
```

It must not yet say:

```text
Gain GREYFEN_TRUST. Supplies cost 5 gp less.
```

Exact tag creation and implementation belong to later sequence-planning and verification stages.

## 6D. `STORY_TAGS.md` initial format

If `[Book]/STORY_TAGS.md` does not exist, create:

```markdown
# STORY TAGS

This file is the production registry for persistent player-facing gameplay tags.

Persistent tags are rare state used only when an earlier outcome needs to affect later survival risk or material outcome after branches reconverge.

They are not XP, achievements, canonical facts, or ordinary character-sheet state.

## Registry Rules

- Core story information and mandatory progression must not depend on an optional persistent tag.
- A persistent tag must have a clear later survival or material effect.
- Do not create a tag when HP, money, equipment, consumables, conditions, spell slots, or another ordinary rule already represents the state.
- Prefer a small number of reused tags over many one-off tags.
- Prefer positive tags with absence representing the default state.
- Exact tag effects must be verified before player-facing use.

## Persistent Tags

[Tag records created by later authorised planning/verification stages.]
```

If `STORY_TAGS.md` already exists:

- preserve existing tag names and meanings;
- do not silently delete or repurpose later-stage tags;
- do not create exact new tag records at Prompt 0122 merely because a chapter might benefit from one;
- flag contradictions between the chapter plan and existing tags.

---

# 7. Extract existing canon before designing chapters

Before inventing any new chapter-level development, extract the book-level canonical facts already established by `BOOK_CONCEPT.md`.

Record in `FACTS.md` only facts likely to matter to later story planning, continuity, or player knowledge.

Do not register:

- stylistic instructions;
- prose preferences;
- encounter budgets;
- file paths;
- prompt rules;
- general genre expectations;
- every descriptive adjective.

Examples of facts worth registering include:

- central objective;
- major named people;
- important relationships;
- known starting location;
- known threat;
- established history;
- central mystery;
- explicit destination;
- explicit antagonist or cause;
- facts the player knows at the beginning;
- facts explicitly hidden at the beginning.

When copying a fact from `BOOK_CONCEPT.md`, preserve its meaning exactly.

Do not strengthen ambiguity into certainty.

For example:

If the concept says:

> Villagers suspect the bell comes from the drowned ruins.

do **not** register:

> The bell comes from the drowned ruins.

unless the concept separately establishes that as objective truth.

---

# 8. Task

Create exactly **6 short chapter overviews** for the adventure.

Each chapter overview must define:

- what happens narratively;
- what role the chapter plays in the overall adventure;
- the **guaranteed story progress** that all viable continuing routes can obtain;
- the chapter's broad **survival pressure**;
- the chapter's broad **material outcome opportunities or costs**;
- any high-level **persistent story-tag opportunity**, or `None`;
- the exact mix of encounter/resolution types that should appear in that chapter;
- the common narrative state that hands off naturally to the next chapter, or resolves the book in Chapter 6.

Keep the overviews concise.

Include only information needed by later planning stages.

Do not design sequences or individual entries yet.

Do not specify:

- exact clues or evidence;
- exact individual rewards;
- exact persistent tag identifiers;
- exact tag effects;
- numerical mechanics;
- exact DCs;
- exact encounter statistics;
- finished prose.

---

# 9. Primary gameplay model — survival and material outcome

For this book, treat **survival** and **material outcome** as the two primary gameplay-value axes.

## 9A. Survival

Survival means whether the player can continue the adventure and ultimately reach a successful ending alive.

HP, conditions, combat resources, hazards, dangerous routes, safer routes, rest opportunities, and similar mechanics are **intermediate survival pressure**.

The goal is not to keep the player untouched. A player who finishes on 1 HP still succeeded.

Design survival pressure so that:

- risk matters;
- failure can make later survival harder;
- success can preserve HP/resources or avoid danger;
- danger escalates appropriately;
- ordinary 5e mechanics carry most of this load.

## 9B. Material outcome

Material outcome means what useful material value the player gains, preserves, spends, or loses.

This may include:

- coin;
- useful mundane equipment;
- consumables;
- limited special items;
- discounts;
- reduced costs;
- preserved supplies;
- access to optional material rewards.

Material outcome is distinct from story information.

## 9C. Story information normally advances

**Core story information required to continue the adventure should normally remain available on every viable outcome.**

Do not make a successful roll the only way to learn a mandatory clue or obtain information needed for core progression.

Instead, checks and choices should normally change the **price of progress**.

At later detailed stages, the preferred pattern is:

```text
SUCCESS:
progress + lower survival risk and/or better material outcome

FAILURE:
progress + higher survival risk and/or worse material outcome
```

Examples at a high level:

- a successful social approach may later earn a discount or safer assistance;
- a failed social approach may still provide the needed information but without those benefits;
- successful investigation may reveal the mandatory fact plus a safer route;
- failed investigation may reveal the mandatory fact but leave the dangerous route as the only immediate option;
- successful exploration may preserve resources or expose optional loot;
- failed exploration may still advance but cost HP, supplies, time, or position.

Prompt 0122 must not design these exact entry-level outcomes. It must ensure each chapter has enough **survival and material stakes** for later stages to build meaningful checks and choices.

## 9D. Information is progression, not the default reward

Do not use required story information itself as the primary reward for success.

Optional information may still exist when it enriches play, but mandatory narrative progress should not depend on one successful check.

A difference in:

- tone;
- NPC warmth;
- descriptive flavour;
- wording;

is not by itself a meaningful gameplay consequence.

Later stages must convert important interactions into survival/material differences or remove unnecessary rolls.

---

# 10. Adventure progression

The six chapters must form **one coherent, escalating adventure**.

Chapters are internal planning units. They must not require:

- visible chapter headings;
- artificial endings;
- chapter-boundary announcements;
- player-facing production terminology.

Only the final chapter ends the adventure.

The others hand off naturally to the next part of the story.

The encounter mix should evolve with the story.

In general:

- early chapters should favour exploration, investigation, player decisions, and skill use;
- combat should be relatively uncommon early unless strongly justified by the story;
- danger and mechanical pressure should increase through the adventure;
- later chapters may contain substantially more combat, saving throws, and dangerous mechanical outcomes;
- the final chapters should feel more dangerous and climactic;
- non-combat choices and skill-based approaches should remain important throughout the book.

Do not force encounters into a chapter purely to satisfy variety.

The allocation should make narrative sense.

---

# 11. Chapter-level fact discipline

Before finalising each chapter, explicitly distinguish:

### Already established

Facts that come from:

- `BOOK_CONCEPT.md`;
- existing `FACTS.md`;
- or the previous chapter's common handoff.

### New canon deliberately established by this chapter plan

Only high-level story facts necessary to define the chapter's development.

These must be:

- stated in the chapter overview;
- added to `FACTS.md`;
- assigned an appropriate player-knowledge gate.

### Still unresolved

Important questions that this chapter does not answer.

Do not accidentally answer them in the overview.

If a later chapter is supposed to reveal something, earlier chapter text must not treat it as player knowledge.

A planned future truth may be recorded as objective canon in `FACTS.md` with:

```text
Player knowledge: Hidden
```

or:

```text
Player knowledge: Planned common reveal: Chapter X
```

This is allowed only when Prompt 012 has deliberately made that high-level truth part of the book design.

Do not create detailed supporting evidence for that truth here.

---

# 12. Character advancement — milestones only

The adventure uses **milestone advancement, not XP**.

- The player starts at the **single starting level specified in `BOOK_CONCEPT.md`**.
- Design the **entire book** for a character at that starting level.
- Do not assume the character gains levels during the adventure.
- On **successful completion of the book**, the character advances **exactly one level**, from the specified starting level to starting level + 1.
- Do not award levels at chapter or sequence boundaries.
- No partial level-ups, XP awards, XP totals, XP checkpoints, or XP-bearing tags exist.
- A premature or unsuccessful ending does not automatically grant the completion milestone.
- The book's final successful endings must make the milestone unambiguous.
- Ensure the final chapter resolves the central conflict and provides the adventure's genuine conclusion.
- Do not manufacture a level-up scene earlier.

### Series support

If the book concept identifies this as part of a series, a completed character may carry their level and permitted possessions into the next book.

For example, three successive books could start at levels 1, 2, and 3 and end at levels 2, 3, and 4 respectively.

Do not assume every series starts at level 1.

Do not set the next book's level or rewards unless its concept establishes them.

Each book must remain playable at its stated starting level.

---

# 13. Encounter / resolution types

Every numbered entry will later be assigned one primary resolution type.

Use these categories:

**Player Choice**  
The player deliberately chooses between meaningful actions, routes, approaches, or responses.

**Ability / Skill Check**  
Progress or consequences are determined by an ability or skill check.

**Saving Throw**  
The player must resist or survive a danger, effect, hazard, or other threat.

**Combat**  
The entry contains a combat encounter whose outcome matters to progression.

**Other Mechanical Outcome**  
A rules-driven challenge or resolution that does not fit the categories above.

**Automatic Transition**  
A narrative, discovery, consequence, or transition entry that proceeds without a player decision or mechanical test.

---

# 14. Encounter budget

For each chapter, assign an exact number of entries to every resolution type.

The numbers must add up **exactly to 68 entries**, matching the fixed five-sequence chapter architecture.

Treat these numbers as planning quotas.

The fixed chapter total is not configurable. The later five sequence plans will contain exactly:

- Sequence 1: 12 entries
- Sequence 2: 14 entries
- Sequence 3: 16 entries
- Sequence 4: 14 entries
- Sequence 5: 12 entries

Together they must implement the same **68-entry chapter encounter budget** without adding or removing entries.

They will later be divided between the chapter's sequences and ultimately implemented as individual entries.

Do not specify:

- DCs;
- CRs;
- monster statistics;
- damage;
- individual material rewards or treasure values;
- individual tag identifiers;
- detailed rules.

Those belong to later planning stages.

Do not add challenges merely to create rewards or meet an imagined XP quota.

Do not budget checks, saves, combat, or other mechanical outcomes merely for variety. At later stages, each consequential interaction must have a credible survival/material stake. If a proposed interaction cannot affect survival, material outcome, or a persistent tag that later affects one of those axes, prefer a meaningful player choice or Automatic Transition rather than a fake roll.

---

# 15. Minimal bookkeeping and state

The adventure should be easy to play with:

- a character sheet;
- ordinary inventory;
- a small number of persistent story tags;
- very little additional tracking.

Do **not** create achievement tags to record:

- every successful check;
- every failed check;
- every defeated monster;
- every discovery;
- every choice;
- every reward.

Persistent story tags are different. They may survive across sequences or chapters when they have a clear later effect on **survival risk** or **material outcome**.

Use three distinct kinds of state:

1. **Ordinary character state** — HP, money, equipment, spell slots, consumables, conditions, and similar rules; track normally.
2. **Temporary local state tags** — zero by default; at most two distinct tags inside a single sequence when topology genuinely needs short-term memory; these must resolve within that sequence.
3. **Persistent story tags** — rare player-facing state registered in `STORY_TAGS.md`; may cross sequence/chapter boundaries only when they have a planned later survival/material effect.

Do not create a persistent story tag when ordinary character state already represents the consequence.

Do not require combinations of persistent story tags for mandatory progression.

Core progression must work through planned entry connections, guaranteed story information, and common narrative handoffs.

`FACTS.md` fact IDs are **not game-state tags** and are never tracked by the player.

At this chapter-overview stage:

- do not create temporary local tags;
- do not create exact persistent tag identifiers;
- identify only high-level persistent-tag opportunities;
- ensure any such opportunity has a plausible later survival/material use.

Use fail-forward where appropriate. Failure should normally preserve core story progress while increasing survival pressure, reducing material benefit, or forfeiting an optional persistent advantage.

---

# 16. Material outcomes — meaningful and purposeful

Material outcome is one of the book's two primary gameplay-value axes.

The player should care not only about surviving, but about what useful material value they gain, preserve, spend, or lose along the way.

Material value may include:

- modest coin;
- useful mundane equipment;
- consumables;
- occasional setting-appropriate special items;
- preserved supplies;
- discounts or reduced costs;
- access to optional loot opportunities.

Do not turn the book into a loot-management simulator.

As a **soft book-wide guideline**:

- aim for roughly **2–4 notable lasting material rewards** across the six chapters;
- allow smaller material advantages, discounts, preserved resources, or optional loot opportunities more often when they naturally reward good play;
- do not require a material reward in every sequence;
- do not create loot merely to make every roll feel rewarded.

Follow the book concept if it establishes a different reward profile.

Material outcome may be used to make success and failure meaningful later:

```text
Success:
progress + preserve/gain material value

Failure:
progress + spend/lose/forgo material value
```

Examples include:

- pay less rather than more;
- preserve a consumable rather than expend it;
- gain optional loot rather than miss it;
- avoid losing equipment or supplies;
- reach a reward opportunity through a safer route.

Do not make optional treasure or a particular reward-dependent branch mandatory for the core story.

Any item essential to the main plot must be reliably obtainable and treated as a **plot requirement**, not optional loot.

Do not assume all routes find the same optional treasure.

Later stages must keep rewards and resulting inventory valid for each route.

Do not invent at this stage:

- exact prices;
- exact quantities;
- exact item statistics;
- exact loot locations;
- exact persistent tag identifiers or effects;
- a new currency economy;
- carryover restrictions not established by the concept.

For each chapter include:

```text
Material outcome intent:
```

with either:

```text
None
```

or one concise description of the chapter's likely material stakes/opportunities.

Examples:

```text
A modest payment is possible, while poor social outcomes may reduce later buying power.
```

```text
The player may preserve supplies or gain one useful consumable through successful exploration.
```

Also include:

```text
Persistent tag opportunity:
```

with either:

```text
None
```

or one high-level description of a possible persistent advantage/disadvantage whose later use would affect survival or material outcome.

Example:

```text
Local trust may later provide safer assistance or a discount.
```

Review all six chapters together so material opportunities and persistent tag opportunities are distributed deliberately rather than mechanically.

Ensure any ordinary rewards intended to carry into another book are compatible with the series concept.

---

# 17. Chapter handoffs and final milestone

For Chapters 1–5, describe a **common narrative state** that every continuing valid route can reach and from which the following chapter can begin.

Handoffs must not assume:

- an optional discovery;
- optional reward;
- specific branch;
- possession of an optional persistent story tag.

Persistent story tags may carry across the handoff, but the common narrative state must remain valid whether or not the player has them.

Every handoff fact that later chapters may rely upon must be represented in `FACTS.md` with an appropriate player-knowledge gate.

The player must not see the chapter boundary.

The final entry of each chapter will later link normally to the next numbered entry.

Numbering runs consecutively throughout the book.

For Chapter 6, provide a genuine resolution of the central conflict.

Its **successful completion** triggers the single milestone level-up.

Do not place the level-up at a routine sequence/chapter transition or award it for an unsuccessful ending.

---

# 18. `FACTS.md` update after chapter planning

After all six chapter overviews are drafted, perform a dedicated canon pass.

## 16A. Register all new Prompt 012 canon

For each new high-level story fact introduced in `CHAPTER_OVERVIEWS.md`:

1. decide whether it is truly canonical or merely a planning description;
2. if canonical, add or update a `FACT-[NNNN]` record;
3. cite `CHAPTER_OVERVIEWS.md` and the exact chapter as its source;
4. assign its correct player-knowledge state;
5. do not create duplicate records for the same fact.

Examples of facts likely to belong:

- a major location definitely exists;
- a named NPC definitely has a defined role;
- a chapter establishes a definite cause;
- the player will commonly learn a specific truth;
- the central conflict has a particular actual resolution.

Examples that usually do not belong:

- "Chapter 2 is investigation-heavy";
- "there should be several choices";
- "danger increases";
- "the chapter has 30 entries";
- "the player crosses difficult terrain" when no later continuity depends on the exact terrain.

## 16B. Record important unresolved matters

Under:

```markdown
## Not Yet Established / Must Not Be Assumed
```

record only important unresolved matters that later stages might otherwise be tempted to fill in.

Examples:

- identity of an unknown figure;
- exact cause of an unexplained event;
- who created a particular object;
- the meaning of a clue not yet defined;
- the method by which a later planned conclusion will be proven.

Do not fill this section with trivial unknowns.

If a matter is deliberately resolved by the new chapter overview, remove or update the corresponding unresolved note.

## 16C. No retroactive player knowledge

A fact may be objectively true from the beginning of the story while still hidden from the player.

Do not mark it as common player knowledge before the planned reveal.

Later stages may make the reveal point more precise, for example:

```text
Planned common reveal: Chapter 2
```

becoming:

```text
Common after published entry 80
```

but they must not reveal it earlier without an authorised upstream revision.

---

# 19. Output format

At the beginning of `[Book]/CHAPTER_OVERVIEWS.md`, record:

```markdown
# CHAPTER OVERVIEWS

**Starting character level:** [from BOOK_CONCEPT.md]  
**Advancement:** One milestone level on successful completion of Chapter 6; no advancement earlier  
**Finishing level after successful completion:** [starting level + 1]
```

Then for each chapter output:

```markdown
# Chapter 1 — [Title]

**Purpose:**  
One or two sentences.

**Main developments:**  
A short description of the important events and progression.

**End state / handoff:**  
What has changed by the end and how it leads naturally into the next chapter. For Chapter 6, describe the successful adventure resolution and completion milestone instead.

**Guaranteed story progress:**  
The core information/development every viable continuing route can obtain in this chapter.

**Survival pressure:**  
A brief high-level description of how this chapter can make survival easier or harder without specifying exact entry mechanics.

**Material outcome intent:**  
`None` or a brief high-level description of likely material gains, costs, savings, losses, or optional reward opportunities.

**Persistent tag opportunity:**  
`None` or one brief high-level description of a possible persistent state whose later use would affect survival or material outcome. Do not name the tag or define its exact effect yet.

**Encounter emphasis:**  
A very short description such as "investigation-heavy, low combat" or "danger-heavy with increasing combat."

**Encounter budget — 68 entries:**

- Player Choice: X
- Ability / Skill Check: X
- Saving Throw: X
- Combat: X
- Other Mechanical Outcome: X
- Automatic Transition: X

```

Repeat for all six chapters.

Do not include:

- XP budgets;
- XP checkpoint fields;
- achievement-tag tables;
- fact IDs in player-facing prose;
- detailed evidence;
- individual entry design.

`FACTS.md` is maintained separately as production metadata.

---

# 20. Fact-safety validation

Before saving, perform this dedicated validation.

For every chapter:

- Every new canonical story fact is explicit in the overview rather than hidden in incidental phrasing.
- Every new canonical fact is recorded in `FACTS.md`.
- Every fact copied from `BOOK_CONCEPT.md` preserves the concept's degree of certainty.
- Suspicions, rumours, beliefs, and possibilities are not silently converted into objective truth.
- Facts intended for later revelation are not marked as already known.
- No chapter assumes a fact that belongs only to a later reveal.
- No entry-level clue, evidence, object detail, or exact proof has been invented.
- No unresolved major question has been accidentally answered.
- The handoff contains only common facts intended to be available to every continuing route.
- `FACTS.md` contains no encounter budgets, topology, XP bookkeeping, or prose embellishment.
- Existing downstream facts have not been silently overwritten.
- Core story information required for progression is not planned as success-only content.
- Each chapter has enough plausible survival/material stakes for later prompts to build meaningful consequences.
- Any persistent tag opportunity has a plausible later survival/material use.
- No exact persistent tag identifier or exact mechanical tag effect has been invented at this stage.
- `FACTS.md` and `STORY_TAGS.md` remain separate: facts are truth; tags are persistent gameplay state.

If a chapter requires unsupported specificity, revise it back to the highest level actually authorised.

If the missing specificity is essential to the book's chapter-level story and cannot be safely decided under Prompt 012's authority, stop and identify the upstream decision required.

---

# 21. Requirements

- Create exactly 6 concise chapter overviews forming one coherent escalating adventure.
- Give each chapter a distinct narrative purpose and preserve room for meaningful branching and fail-forward.
- Assign one exact encounter budget per chapter, with all six resolution-type counts adding up to exactly **68 entries**.
- Vary encounter mixes according to the story; generally increase danger toward the later chapters without forcing combat.
- Read the starting character level from `BOOK_CONCEPT.md`; do not guess it or ignore a contradiction in the concept.
- Keep the character at that **single starting level throughout the book**.
- Award **exactly one milestone level on successful completion only**.
- Do not plan XP, XP checkpoints, per-challenge advancement, XP-bearing achievement tags, or an achievement-tag registry.
- Treat **survival** and **material outcome** as the two primary gameplay-value axes.
- Keep core story information and mandatory progression available on every viable route; success should normally improve the price of progress rather than be the only way to progress.
- Plan enough survival/material stakes that later checks and choices can create meaningful up/down consequences.
- Use ordinary character-sheet/inventory tracking for HP, money, equipment, consumables, conditions, spell slots, and similar state.
- Default temporary local state to zero; later sequence planning may use at most two distinct local tags when topology genuinely requires short-term memory.
- Do not design temporary local tags here.
- Permit rare persistent story tags only when they can later change survival risk or material outcome.
- Do not define exact persistent tag identifiers or exact effects here; identify only high-level opportunities.
- Do not make any persistent tag or optional reward necessary for mandatory progression or required story information.
- Plan roughly 2–4 notable lasting material rewards across the book as a soft guideline, while allowing smaller material advantages, discounts, preserved resources, or optional loot opportunities where natural.
- Do not define exact loot here.
- Keep chapters and sequences invisible to the player, with natural narrative handoffs and consecutive book-wide entry numbers.
- Do not design sequences, individual entries, numerical game mechanics, or finished prose.
- Avoid unnecessary lore or explanation.
- Chapter 6 must resolve the central conflict, conclude the adventure, and identify the successful-completion milestone.
- Create and maintain `[Book]/FACTS.md` as canonical production memory.
- Create `[Book]/STORY_TAGS.md` if absent and preserve existing valid persistent story-tag records.
- Keep `FACTS.md` and `STORY_TAGS.md` conceptually separate: facts record truth; tags record rare persistent gameplay state.
- Never introduce a canonical fact silently.
- Every Prompt-012-created canonical fact must be explicit in `CHAPTER_OVERVIEWS.md` and registered with provenance and player-knowledge timing.
- Do not invent exact evidence or entry-level proof for chapter-level conclusions.
- Preserve uncertainty exactly when upstream sources are uncertain.
- Record important unresolved matters so later prompts know what they must not assume.

---

# 22. Output

Save the chapter overview as:

```text
[Book]/CHAPTER_OVERVIEWS.md
```

Create or update the canonical fact registry as:

```text
[Book]/FACTS.md
```

Create `[Book]/STORY_TAGS.md` if it does not exist:

```text
[Book]/STORY_TAGS.md
```

At Prompt 0122, `STORY_TAGS.md` should normally contain registry rules and any valid pre-existing tags, but **no newly invented exact tag records**. Exact persistent tags are created only by later authorised planning/verification stages when their acquisition and later survival/material use can be defined.

For `FACTS.md`:

- preserve existing stable fact IDs;
- append new IDs monotonically;
- do not renumber;
- do not silently delete later-stage facts;
- do not overwrite downstream canon without explicit upstream revision authority.

For `STORY_TAGS.md`:

- preserve existing valid persistent tag names and meanings;
- do not silently repurpose or delete later-stage tags;
- do not create exact new tags merely because a chapter overview identifies a high-level tag opportunity;
- keep core story progression independent of optional tags.

Confirm that the six chapter directories exist when directory creation is available.

Otherwise report that they still need to be created.

Do not create any other files or folders for this stage.

When complete, report briefly:

- that `CHAPTER_OVERVIEWS.md` was written;
- that `FACTS.md` was created or updated;
- that `STORY_TAGS.md` was created or preserved;
- how many canonical facts were added or updated by this stage;
- which chapters, if any, contain high-level persistent-tag opportunities;
- whether any important unresolved facts were recorded;
- any conflict that prevented safe completion.
