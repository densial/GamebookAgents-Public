# 022 — CHAPTER TO SEQUENCES

## Inputs

The user will provide:

- **Book**
- **Chapter number**

## Workspace layout and path resolution

This reusable prompt lives under `[Workspace]/builder/prompts/`.

The shared builder root is:

```text
[Workspace]/builder/
```

Each book is an independent sibling directory.

`[Book]` means the existing book root supplied by the user, either as an absolute path or a path relative to `[Workspace]`.

Keep reusable prompts, topology files, and rules sources under the builder.

Write book-specific planning files only under `[Book]`.

Use the Book value to locate the correct book directory.

Do not create a second nested folder with the same book name.

Read:

```text
[Book]/BOOK_CONCEPT.md
[Book]/CHAPTER_OVERVIEWS.md
[Book]/FACTS.md
[Book]/STORY_TAGS.md
```

Use only the specified chapter from `CHAPTER_OVERVIEWS.md`, plus book-level information required for:

- starting level;
- advancement;
- series continuity;
- chapter encounter budget;
- material-outcome constraints;
- persistent-tag opportunity;
- facts whose meaning is genuinely ambiguous without the book concept.

`FACTS.md` is the authority for established canonical story truth and player-knowledge timing.

`STORY_TAGS.md` is the authority for already-established persistent gameplay tags.

Persistent tags are rare player-facing state that may affect later **survival risk** or **material outcome**.

They are not facts, XP, achievements, or a record of every check.

If `FACTS.md`, `STORY_TAGS.md`, the chapter overview, starting level, or advancement model is missing or contradictory, stop and report the issue.

Do not invent:

- a character level;
- an XP scheme;
- additional advancement;
- a replacement chapter premise;
- facts that contradict established canon.

---

# 1. Role of Prompt 022

Prompt 022 is a **chapter sequence-architecture proposal stage**.

Its job is to divide one approved chapter into five strong sequence-scale units.

It designs.

It does **not** approve its own architecture.

It does **not** write permanent sequence-level canon into `FACTS.md`.

It does **not** create exact persistent tags.

Prompt 023 will independently verify the five-sequence architecture, adjudicate any sequence-level canon proposals, and update `FACTS.md`.

The downstream flow is:

```text
012 — book/chapter architecture and initial registries
022 — propose five-sequence chapter architecture
023 — verify/approve chapter sequence architecture and sequence-level canon
032 — detailed entry-by-entry sequence planning
033 — structural/playability verification
034 — entry-level canon + persistent-tag adjudication
042 — numerical mechanics
```

Do not bypass Prompt 023.

---

# 2. Book folder structure

Follow the directory convention established by Prompt 012:

```text
[Book]/
├── BOOK_CONCEPT.md
├── FACTS.md
├── STORY_TAGS.md
├── CHAPTER_OVERVIEWS.md
├── chapters/
│   ├── chapter-01/
│   │   ├── sequences.md
│   │   ├── sequence-01/
│   │   │   ├── plan.md
│   │   │   ├── mechanics.md
│   │   │   ├── final.md
│   │   │   └── prose-validation.md
│   │   └── ... sequence-05/
│   └── ...
└── output/
```

For this stage only:

- create/reuse `[Book]/chapters/chapter-[NN]/`;
- write `[Book]/chapters/chapter-[NN]/sequences.md`;
- read `FACTS.md` but do not modify it;
- read `STORY_TAGS.md` but do not modify it;
- optionally create empty `sequence-01/` through `sequence-05/` directories if supported.

Do not create mechanics, prose, publication output, or another chapter's sequence content.

If rerunning Prompt 022 would invalidate existing downstream plans, mechanics, or prose for this chapter, stop and identify those files before overwriting `sequences.md`.

---

# 3. Fixed five-sequence architecture

Every chapter contains exactly five sequences.

Their later entry capacities are fixed:

| Sequence | Later entry capacity |
|---|---:|
| 1 | 12 |
| 2 | 14 |
| 3 | 16 |
| 4 | 14 |
| 5 | 12 |

Total:

```text
68 entries per chapter
```

Prompt 022 does **not** assign individual entries or topology variants.

However, it must design enough substantive situation development and gameplay leverage to make each capacity plausible.

Do not create a 12-, 14-, or 16-entry sequence whose entire content is one small conversation, one cosmetic choice, one clue followed by transitions, or repeated restatement of the same situation.

A sequence overview must contain enough **distinct playable payload** that Prompt 032 can later map it onto the fixed topology without padding.

---

# 4. Canon authority and proposal model

`FACTS.md` is the authoritative narrative registry.

Prompt 022 is read-only for it.

Prompt 022 may need to propose a new **sequence-scale canonical fact** to divide an approved chapter into coherent stages.

Examples include:

- which broad location becomes the next destination;
- which high-level chapter revelation becomes common in a sequence;
- which broad obstacle is resolved before the next sequence;
- which sequence establishes a planned chapter-level truth.

Prompt 022 must not invent exact evidence, object, witness wording, trail sign, inscription, bloodstain, footprint, letter, clue placement, sensory cue, combat setup, or mechanical trigger that proves the fact.

Those are entry-level implementation details for later stages.

Any new sequence-scale canonical fact must be proposed explicitly under:

```markdown
## Sequence-Level Canon Proposals
```

Do not write it to `FACTS.md`.

---

# 5. Sequence-Level Canon Proposal format

Use local proposal IDs:

```text
SCF-01
SCF-02
SCF-03
```

These IDs exist only inside this chapter's draft `sequences.md`.

They are not permanent FACT IDs.

For each proposal use:

```markdown
### SCF-01

**Proposal type:** [New fact / Reveal-timing refinement]
**Proposed fact:** [one concise factual statement]
**Existing FACT ID:** [FACT-NNNN or None]
**Implements:** [exact chapter development / existing FACT / book-level authority]
**Planned sequence:** [1–5]
**Player knowledge:** [Common / Optional / Hidden / Not player-facing]
**Planned establishment point:** [Sequence N handoff / during Sequence N / other high-level point]
**Continuity scope:** [This chapter / Later book continuity / Other]
**Reason required:** [why the sequence architecture needs this fact]
**Conflicts checked:** [relevant FACT IDs / unresolved matters]
**Status:** PENDING PROMPT 023
```

Prompt 023 may approve, narrow, treat as a reveal-timing refinement, reject, or require upstream revision.

Prompt 022 must not decide that itself.

If no sequence-level canon proposals are required:

```markdown
## Sequence-Level Canon Proposals

None.
```

---

# 6. Canon safety

Before designing the sequences:

1. identify facts already known at chapter start;
2. identify facts planned for revelation in this chapter;
3. identify route-specific optional knowledge that must not become common;
4. identify matters under `## Not Yet Established / Must Not Be Assumed`;
5. preserve uncertainty exactly.

Do not turn rumour into fact, suspicion into certainty, possibility into cause, or a planned future reveal into current knowledge.

Do not create a sequence-scale fact merely because it makes the outline easier.

If an approved chapter development can remain high-level without defining a new fact, keep it high-level.

---

# 7. Persistent gameplay tags

`STORY_TAGS.md` stores rare persistent player-facing gameplay state.

Prompt 022 may:

- recognise an already-existing persistent tag that can matter in this chapter;
- identify a **high-level opportunity** for a new persistent state where the chapter overview authorises one;
- identify at least one plausible later **payoff window** for that opportunity.

Prompt 022 must not:

- invent an exact new tag identifier;
- create a new tag record;
- define exact numerical effects;
- define exact prices, bonuses, DC changes, route numbers, or item values;
- require an optional tag for mandatory progression.

Exact tag creation and acquisition belongs to Prompt 032.

Exact tag adjudication and registry updates belong to Prompt 034.

Keep tag opportunities sparse.

A chapter will normally introduce 0–1 new persistent-tag opportunity, occasionally 2 when both have clear, distinct later survival/material uses.

---

# 8. Primary gameplay model

For this book, the two primary gameplay-value axes are:

1. **Survival**
2. **Material outcome**

Mandatory story progress should normally remain available on every viable continuing outcome.

The preferred detailed pattern later is:

```text
SUCCESS:
story progresses + lower survival risk and/or better material outcome

FAILURE:
story progresses + higher survival risk and/or worse material outcome
```

Persistent tags are allowed only when they later affect one or both axes.

Relevant survival leverage includes HP/resources, conditions, combat exposure, safer/dangerous routes, rest, tactical position, and useful assistance that materially changes danger.

Relevant material leverage includes coin, equipment, consumables, optional loot, discounts, higher costs, preserved/lost supplies, and later material opportunities.

Information required to continue is normally **progression**, not the reward for succeeding.

---

# 9. Meaningful Player Choice rule

A later entry classified as **Player Choice** must represent meaningful gameplay.

A Player Choice is meaningful only when the options differ in at least one of:

- survival risk;
- survival-resource preservation/expenditure;
- material outcome;
- acquisition/use of an approved persistent state that later affects survival/material outcome;
- an immediately different mechanical challenge whose outcome affects one of those axes.

The following are **not enough by themselves**:

- public versus private tone;
- conversational framing;
- which topic is mentioned first;
- warmer versus colder NPC reaction;
- wording;
- descriptive flavour;
- roleplaying expression with no gameplay consequence.

Such expressive choices may still appear inside prose later.

They do **not** justify a sustained topology branch and should not be relied upon as a core Player Choice encounter.

A branch that persists across multiple later entries must never exist solely for flavour.

---

# 10. Zero-stakes sequence rule

A sequence may be safe, low-mechanics, or contain no combat.

But every later approved sequence topology contains meaningful branching.

Therefore a sequence with:

```text
Survival stakes: None
Material stakes: None
Persistent tag opportunity/use: None
```

must still contain some other authorised survival/material leverage through its planned interactions.

If there is genuinely **no survival, material, or persistent-state gameplay leverage at all**, the sequence is under-specified for the fixed branching architecture.

Do not solve that by inventing cosmetic Player Choices.

Do not write:

```text
Encounter emphasis: choice-led
```

for a sequence whose choices have no gameplay consequence.

Redesign within the chapter's authorised stakes or stop and report an upstream architecture conflict.

---

# 11. Meaningful interaction opportunities

For every sequence include:

```markdown
**Meaningful interaction opportunities:**
```

Describe the high-level interactions that later planning can turn into real Player Choices, checks, saves, combat, or other mechanical outcomes.

For each opportunity state the intended gameplay value in high-level terms.

Examples:

```text
- Social handling: mandatory information is obtained either way, but better handling may preserve buying power or earn later safer assistance.
- Route selection: both routes continue, but one risks more survival resources while the other risks material cost.
- Investigation: the required conclusion is reached, but success may preserve resources or expose optional material value.
```

Do not specify exact checks, DCs, damage, items, prices, or tag names.

Most sequences should support roughly **2–4 meaningful interaction moments**.

This is a guideline, not a quota.

Do not manufacture filler.

However, **zero meaningful interaction opportunities is invalid** for the current fixed branching topology system.

If fewer than two can be identified, flag that for Prompt 023 to scrutinise closely.

---

# 12. Task

Divide the specified chapter into **exactly 5 distinct sequential adventure sequences**.

Together, the five sequences must fully deliver the approved chapter overview.

For each sequence include:

**Entry capacity:**  
12 / 14 / 16 / 14 / 12.

**Purpose:**  
What the sequence accomplishes.

**Main development:**  
The principal situation, event, discovery, obstacle, or progression.

**Guaranteed story progress:**  
The core information/development every viable continuing route can obtain.

**Chapter advancement:**  
How the chapter moves toward its objective.

**Survival stakes:**  
A high-level survival pressure/opportunity. Use `None` only when credible survival/material leverage still exists elsewhere in the sequence and the design is not relying on cosmetic branching.

**Material stakes:**  
A high-level material gain/cost/saving/loss/opportunity, or `None`.

**Meaningful interaction opportunities:**  
Usually 2–4 high-level opportunities. Explain what survival/material value could differ.

**Persistent tag opportunity:**  
`None` or the chapter-approved high-level persistent-state opportunity. Do not name the exact new tag.

**Persistent tag payoff window:**  
`None` or where the new opportunity could later produce survival/material value. The payoff opportunity must lie on a guaranteed reachable future path; using the benefit may remain optional.

**Existing persistent tag relevance:**  
`None` or an already-established tag from `STORY_TAGS.md`.

**Encounter emphasis:**  
The intended balance and character of Player Choices, checks, saves, combat, other outcomes, and transitions. Do not assign exact counts yet.

**Handoff:**  
The common narrative state from which the next sequence safely begins, or Sequence 5's approved chapter resolution.

Do not design individual entries, exact branches, topology selection, exact encounter counts, numerical mechanics, exact items, exact evidence, exact tag identifiers/effects, or finished prose.

---

# 13. Sequence payload and capacity

Each sequence must contain enough distinct development to support its fixed later capacity.

At a high level it should support:

- an opening situation;
- multiple meaningful interactions;
- intermediate developments/consequences;
- route-compatible progress;
- a genuine handoff development.

Do not stretch one small event across 12–16 entries.

Do not reserve all meaningful gameplay for Sequence 5.

Different sequences may have different densities and tones.

What is not acceptable is a sequence whose branching would exist only to consume topology nodes.

---

# 14. Encounter-budget feasibility

Read the chapter encounter budget from `CHAPTER_OVERVIEWS.md`.

Prompt 022 does not assign exact sequence counts.

It must nevertheless ensure the five sequence emphases make the full chapter budget feasible.

Check that:

- Player Choices are associated with real gameplay stakes;
- checks are associated with success/failure that can alter survival/material outcome;
- saves occur where hazards genuinely exist;
- combat occurs only where narratively supported;
- automatic transitions have real narrative/payoff functions;
- the full chapter budget still appears achievable across 68 entries.

Do not use Player Choice as a synonym for "the reader gets to express a preference."

Do not plan a check whose only likely difference is tone.

---

# 15. Handoffs and progression

The five sequences form:

```text
Sequence 1 → Sequence 2 → Sequence 3 → Sequence 4 → Sequence 5
```

Every continuing route must eventually reach the common handoff.

A handoff must:

- use facts true for all continuing routes;
- not require an optional clue;
- not require optional loot;
- not require a particular past branch;
- not require an optional persistent tag;
- match player-knowledge timing.

Persistent tags may carry across a handoff.

The common narrative state must remain valid with or without them.

If a plot-critical item or fact is required, ensure it is reliably available on every continuing route.

Sequence 5 hands naturally to the next chapter, except Chapter 6 Sequence 5, which supports the approved successful book ending.

---

# 16. Character advancement

Use the single starting character level established upstream.

The character remains at that level throughout all five sequences.

No sequence awards XP or a level.

The book awards exactly one level only upon successful completion of the entire adventure.

Only Chapter 6 Sequence 5 may support that actual completion milestone.

Do not create XP budgets, XP tags, XP checkpoints, intermediate level-ups, or advancement counters.

---

# 17. Minimal bookkeeping

Use three kinds of state only:

1. ordinary character state;
2. temporary local state used later only when genuinely necessary;
3. rare persistent story tags used for later survival/material consequences.

At Prompt 022:

- do not name temporary local tags;
- do not create exact new persistent tags;
- do not create achievement flags;
- do not duplicate ordinary character state;
- do not require tag combinations for core progression.

---

# 18. Output format

Write:

```markdown
# Chapter [NUMBER] — Sequences

**Starting character level:** [upstream level]
**Advancement:** One milestone level on successful completion of the book only; none at sequence/chapter boundaries

## Sequence 1 — [Title]

**Entry capacity:** 12

**Purpose:**  
[...]

**Main development:**  
[...]

**Guaranteed story progress:**  
[...]

**Chapter advancement:**  
[...]

**Survival stakes:**  
[...]

**Material stakes:**  
[...]

**Meaningful interaction opportunities:**  
- [...]
- [...]

**Persistent tag opportunity:**  
[...]

**Persistent tag payoff window:**  
[...]

**Existing persistent tag relevance:**  
[...]

**Encounter emphasis:**  
[...]

**Handoff:**  
[...]

## Sequence 2 — [Title]

**Entry capacity:** 14

[repeat]

## Sequence 3 — [Title]

**Entry capacity:** 16

[repeat]

## Sequence 4 — [Title]

**Entry capacity:** 14

[repeat]

## Sequence 5 — [Title]

**Entry capacity:** 12

[repeat]

## Sequence-Level Canon Proposals

[SCF records or None.]

## Prompt 022 Draft Check

**Exactly five sequences:** YES / NO  
**Fixed capacities 12/14/16/14/12:** YES / NO  
**Chapter overview fully covered:** YES / NO  
**Guaranteed story progress defined for every sequence:** YES / NO  
**Meaningful interaction opportunities identified for every sequence:** YES / NO  
**No sustained flavour-only Player Choice architecture intentionally planned:** YES / NO  
**Chapter encounter budget appears feasible:** YES / NO  
**Persistent-tag opportunities sparse and payoff windows identified:** YES / NO  
**No exact entry-level evidence invented:** YES / NO  
**All new sequence-scale canon exposed as SCFs:** YES / NO  
**FACTS.md left unchanged:** YES / NO  
**STORY_TAGS.md left unchanged:** YES / NO

`PROMPT 022: DRAFT COMPLETE — READY FOR PROMPT 023`
```

The Draft Check is a completeness check only.

It is **not approval**.

---

# 19. Prompt 022 draft-quality rules

Before saving, make the draft internally coherent.

Check that:

- every sequence has a distinct purpose;
- every sequence advances the chapter;
- required story information is not success-only;
- sequence payload matches its later capacity;
- interaction opportunities are not tone-only;
- sequences expected to contain choices have credible survival/material consequences;
- persistent tag opportunities are sparse and have reachable payoff windows;
- optional tags are not required by handoffs;
- no exact tag identifiers are invented;
- no exact clues/proofs are invented;
- no unresolved mystery is silently answered;
- no future reveal is treated as current knowledge;
- SCFs are explicit and pending Prompt 023;
- the chapter encounter budget remains feasible.

If a sequence cannot support meaningful branch-driving gameplay under the chapter's approved constraints, do not disguise the problem as a "non-evaluative approach choice."

Report the upstream conflict.

---

# 20. Output

Save:

```text
[Book]/chapters/chapter-[NN]/sequences.md
```

Do **not** modify:

```text
[Book]/FACTS.md
[Book]/STORY_TAGS.md
```

Do not save a duplicate chapter-sequences file at the book root.

If the draft cannot be written, return the complete `sequences.md` content and state that it was not saved.

When complete report briefly:

- saved `sequences.md` path;
- number of SCFs proposed;
- which sequences contain high-level persistent-tag opportunities;
- whether any sequence had difficulty supporting meaningful gameplay for its fixed capacity;
- that `FACTS.md` and `STORY_TAGS.md` were read but not modified;
- whether `PROMPT 022: DRAFT COMPLETE — READY FOR PROMPT 023` was reached.
