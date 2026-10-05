# 023 — VERIFY CHAPTER SEQUENCE ARCHITECTURE

## Inputs

The user will provide:

- **Book**
- **Chapter number**

## Workspace layout and path resolution

This reusable prompt lives under `[Workspace]/builder/prompts/`.

`[Book]` means the existing book root supplied by the user.

Read:

```text
[Book]/BOOK_CONCEPT.md
[Book]/CHAPTER_OVERVIEWS.md
[Book]/FACTS.md
[Book]/STORY_TAGS.md
[Book]/chapters/chapter-[NN]/sequences.md
```

Also inspect the shared topology-plan files in:

```text
[Workspace]/builder/topology/
```

for the eligible node-count families:

```text
12-01 / 12-02 / 12-03
14-01 / 14-02 / 14-03
16-01 / 16-02 / 16-03
```

Prompt 023 does **not** select a topology.

It uses the topology families only to verify that the sequence architecture contains enough meaningful branch-driving gameplay to survive later random topology selection.

If required inputs are missing, stop.

---

# 1. Required Prompt 022 state

The requested chapter's `sequences.md` must contain:

```text
PROMPT 022: DRAFT COMPLETE — READY FOR PROMPT 023
```

If it does not:

- do not update `FACTS.md`;
- do not claim verification;
- return the draft to Prompt 022.

Prompt 023 is an independent adversarial review.

Do not accept Prompt 022's Draft Check as proof that the architecture works.

---

# 2. Role of Prompt 023

Prompt 023 has four jobs:

1. verify the five-sequence chapter architecture;
2. correct fixable architecture problems within approved upstream constraints;
3. adjudicate sequence-level canon proposals (`SCF-XX`);
4. update `FACTS.md` only for approved sequence-level canon/reveal timing.

Prompt 023 reads but does **not** modify `STORY_TAGS.md`.

Prompt 023 does not:

- select topology;
- design individual entries;
- create exact clues/evidence;
- create exact persistent tag identifiers;
- define numerical mechanics;
- write prose.

Successful completion ends with:

```text
PROMPT 023: PASS — READY FOR PROMPT 032
```

---

# 3. Fixed sequence capacities

Verify exactly five sequences with capacities:

| Sequence | Required capacity |
|---|---:|
| 1 | 12 |
| 2 | 14 |
| 3 | 16 |
| 4 | 14 |
| 5 | 12 |

Total:

```text
68
```

Do not approve a sequence that obviously lacks enough substantive content for its capacity.

A sequence cannot be justified merely because later prompts could add description.

More prose is not more gameplay.

---

# 4. Topology-family compatibility

Prompt 032 later selects uniformly from the eligible topology variants for the sequence's node count.

Therefore Prompt 023 must verify that each sequence has enough high-level gameplay payload to map credibly onto the **eligible topology family**, not just one convenient future topology.

Prompt 023 must not choose or reserve a topology.

Inspect the eligible topology-plan files and note their:

- branch-driver nodes;
- branch lifespan;
- reconvergence pattern;
- topology-specific interaction-quality rules;
- prohibited cosmetic-branch patterns.

A sequence fails architecture verification if one or more eligible topology variants would force later planning to create:

- flavour-only branch choices;
- meaningless success/failure;
- repeated filler transitions;
- unsupported canon;
- fake persistent tags;

merely to occupy the graph.

If the architecture works only with one particular topology variant, either:

- strengthen the sequence architecture so all eligible variants remain feasible; or
- stop and report that the topology-selection policy itself requires upstream revision.

Do not silently constrain Prompt 032's random pool.

---

# 5. Primary gameplay verification

The two primary gameplay-value axes are:

1. **Survival**
2. **Material outcome**

Persistent tags are valid only when they later affect one or both.

Core story progress should normally remain available on every viable continuing result.

Prompt 023 must reject any architecture whose intended interaction design relies on:

- required information only on success;
- tone-only check outcomes;
- flavour-only sustained branches;
- persistent tags with no later survival/material value.

---

# 6. Mandatory Player Choice test

Any sequence expected to use **Player Choice** as a meaningful resolution type must identify real gameplay consequences.

Valid differences include:

- survival risk;
- survival-resource use/preservation;
- material gain/loss/cost/saving;
- acquisition/use of persistent state with later survival/material value;
- choosing between mechanically distinct challenges whose outcomes alter those axes.

Invalid differences by themselves include:

- public versus quiet social style;
- bell-first versus disappearance-first framing;
- which subject is mentioned first;
- conversational tone;
- wording;
- roleplaying expression;
- descriptive route flavour.

These may appear as prose-level expression later.

They do **not** justify a core Player Choice node.

A sustained branch occupying multiple entries may never exist solely because the player chose a different style of presentation.

---

# 7. Zero-stakes conflict test

For every sequence inspect:

```text
Survival stakes
Material stakes
Persistent tag opportunity
Existing persistent tag relevance
Meaningful interaction opportunities
Encounter emphasis
```

The following combination is a hard warning:

```text
Survival stakes: None
Material stakes: None
Persistent tag opportunity/use: None
Encounter emphasis: Player Choice / branching / checks
```

Do not excuse it by saying:

- "non-evaluative choice";
- "approach preference";
- "social framing";
- "route-specific experience";
- "different tone";
- "the topology itself remembers the choice."

If the sequence has no gameplay-value axis for a sustained branch, it is under-specified.

Correct it only when the approved chapter overview already provides enough authority to identify credible survival/material leverage.

Otherwise stop for upstream revision.

---

# 8. Meaningful interaction density

Each sequence should normally contain roughly **2–4 meaningful interaction opportunities**.

This is not a quota.

However:

- zero is invalid under the current branching topology system;
- one requires strong evidence that the eligible topology family can still be mapped without cosmetic branching;
- multiple opportunities that all express the same consequence are not genuinely distinct.

For each opportunity verify:

- what the player is trying to achieve;
- what story progress is guaranteed;
- what survival/material value may improve;
- what survival/material value may worsen or be forgone;
- whether a persistent tag is genuinely needed;
- whether later mapping can create success/failure or choice outcomes without inventing canon.

Do not accept:

```text
different conversational experience
```

as the gameplay payoff.

---

# 9. Sequence payload test

For each sequence ask:

### Does it have enough development for its capacity?

A valid sequence should contain enough high-level material for:

- opening situation;
- multiple meaningful interactions;
- intermediate changes/consequences;
- route-compatible progression;
- a genuine handoff development.

### Does it repeat itself?

Reject sequences that would require later entries to repeat:

- the same question;
- the same clue;
- the same hazard;
- the same social posture;
- the same outcome;

without escalation or changed function.

### Is its handoff earned?

The handoff must represent a real development, not merely:

```text
the player is now ready for the next sequence
```

unless substantive playable development genuinely occurred first.

---

# 10. Encounter-budget feasibility

Read the chapter encounter budget from `CHAPTER_OVERVIEWS.md`.

Verify that the five sequence emphases make the full 68-entry budget feasible.

Do not assign exact per-sequence counts.

Check that:

- Player Choice demand is supported by meaningful choice opportunities;
- ability/skill-check demand is supported by real success/failure stakes;
- saving throws correspond to actual hazards;
- combat is narratively supported and suitable for the starting level;
- other mechanical outcomes have real game purpose;
- automatic transitions are not being used as padding;
- no sequence is forced to absorb an implausible remainder of the chapter budget.

If later Prompt 032 would obviously be forced to manufacture mechanics to satisfy the budget, the architecture fails now.

---

# 11. Guaranteed story-progress verification

For every sequence verify that:

- mandatory information/progression is available on every viable continuing outcome;
- success may improve survival/material outcome without owning the required clue;
- failure may worsen survival/material outcome without halting progress;
- optional discoveries remain optional;
- optional persistent tags remain optional;
- no handoff requires optional loot or tag ownership.

Fail-forward should be possible without making failure superior to success.

---

# 12. Persistent-tag opportunity verification

Read `STORY_TAGS.md`.

For existing tags verify:

- the sequence does not change their meaning;
- planned relevance fits their registered purpose.

For proposed high-level new tag opportunities verify:

- the chapter overview authorises the opportunity;
- the opportunity is sparse;
- it has a clear later survival/material payoff window;
- that payoff window is on a guaranteed reachable future path;
- possession may be optional, but the opportunity to use the benefit is not purely hypothetical;
- the tag is not duplicating ordinary HP/money/equipment/condition/resource state;
- it is not merely remembering story information.

Prompt 023 must not create the exact tag name or registry record.

That remains for Prompt 032/034.

---

# 13. Handoff verification

For each Sequence 1–4 handoff verify:

- it is true for every continuing route;
- it contains only common story state;
- it does not require an optional clue;
- it does not require optional loot;
- it does not require a persistent tag;
- it does not assume a specific future topology branch;
- it leads naturally into the next sequence's starting situation.

For Sequence 5 verify the approved chapter resolution/handoff to the next chapter.

For Chapter 6 Sequence 5 verify only the already-approved successful-book ending/milestone structure.

Do not create a mini-ending simply because a sequence stops.

---

# 14. Canon proposal adjudication

Read:

```markdown
## Sequence-Level Canon Proposals
```

Every `SCF-XX` must be adjudicated.

## Approval test

An SCF may be approved only if:

- it implements an already-approved chapter/book development;
- it is genuinely sequence-scale rather than entry-scale;
- it does not invent exact evidence/proof;
- it does not contradict `FACTS.md`;
- it preserves uncertainty;
- it does not resolve a protected unresolved matter without authority;
- its player-knowledge timing matches the five-sequence architecture;
- it is necessary or materially useful to continuity;
- it does not exist merely to rescue weak gameplay.

## Outcomes

Choose exactly one:

### APPROVE

The proposal is valid unchanged.

### APPROVE WITH NARROWING

The proposal is valid but more specific than necessary.

Use the minimal sequence-scale fact.

### REVEAL-TIMING REFINEMENT

Use when an existing FACT already contains the truth and only its chapter-level planned reveal needs refinement to a specific sequence.

Do not create a duplicate fact.

### REJECT AND REMOVE

Use when the proposal is unnecessary, unsupported, contradictory, entry-level implementation detail, or outside chapter authority.

Correct `sequences.md` so it no longer depends on the proposal.

### STOP — UPSTREAM DECISION REQUIRED

Use when the chapter cannot remain coherent without a high-level decision beyond Prompt 023's authority.

Do not invent it.

---

# 15. `FACTS.md` update

For approved new SCFs:

- assign the next monotonically increasing `FACT-[NNNN]`;
- preserve all existing IDs;
- never renumber;
- avoid duplicates.

Use the established registry format:

```markdown
### FACT-[NNNN]

**Fact:** [concise canonical statement]  
**Category:** [Character / Relationship / Location / Event / Objective / Cause / World / Item / Other]  
**Source:** chapters/chapter-[NN]/sequences.md — Sequence [N]  
**Player knowledge:** [Known from start / Hidden / Planned common reveal: Chapter X, Sequence Y / Planned optional reveal: Chapter X, Sequence Y / Not player-facing]  
**Established at:** [Start / Planned Chapter X Sequence Y handoff / during planned Sequence Y / other appropriate high-level point]
```

For reveal-timing refinements:

- update timing only;
- do not alter fact substance.

Do not write exact clues, proof, topology metadata, encounter budgets, material mechanics, or tag mechanics into `FACTS.md`.

---

# 16. Rewrite `sequences.md` after canon adjudication

Replace:

```markdown
## Sequence-Level Canon Proposals
```

with:

```markdown
## Verified Sequence-Level Canon
```

If none:

```markdown
## Verified Sequence-Level Canon

None.
```

For approved new facts include:

```markdown
### FACT-[NNNN]

**Fact:** [approved fact]  
**Plan proposal:** SCF-XX  
**Status:** [APPROVED / APPROVED WITH NARROWING]  
**Planned sequence:** [N]  
**Player knowledge:** [Common / Optional / Hidden / Not player-facing]  
**Registry:** [Book]/FACTS.md
```

For reveal-timing refinements include the existing FACT ID and:

```text
Status: REVEAL-TIMING REFINEMENT
```

Remove rejected SCFs from active architecture dependencies.

No `SCF-XX` may remain as an active downstream dependency after Prompt 023.

---

# 17. Architecture correction authority

Prompt 023 should correct fixable problems directly in this chapter's `sequences.md` when doing so stays within:

- `BOOK_CONCEPT.md`;
- the specified chapter overview;
- established `FACTS.md`;
- established `STORY_TAGS.md`;
- chapter encounter budget;
- existing chapter-level survival/material/tag intent.

Examples of authorised corrections:

- strengthen a weak sequence purpose;
- move a high-level interaction opportunity between sequences;
- change an encounter emphasis from choice-heavy to investigation/hazard-heavy;
- make a handoff route-safe;
- remove cosmetic interaction opportunities;
- redistribute chapter-approved material/tag opportunities among the five sequences;
- narrow a sequence-scale canon proposal.

Do not invent a new chapter premise, major reveal, unapproved tag opportunity, exact evidence, topology selection, or individual entries.

If the only fix requires one of those, stop.

---

# 18. Chapter-level adversarial review

Before PASS, ask:

### Could Prompt 032 turn each sequence into an actual game?

Not merely prose.

### Would any sequence still work if all flavour-only choices were removed?

If not, it is too dependent on cosmetic branching.

### Does every sustained branch have a reason for the player to care?

The reason must ultimately touch survival/material gameplay.

### Is any sequence using "low risk" to mean "no consequence"?

Low risk is allowed.

Zero meaningful consequence across a whole branching sequence is not.

### Are tags being used sparingly?

A tag should exist because reconvergence needs persistent gameplay memory, not because tags are available.

### Is information being misused as reward?

Mandatory clues should progress the story.

The gameplay question is the cost/benefit of obtaining that progress.

---

# 19. Verification summary

At the end of the corrected `sequences.md`, append:

```markdown
## Prompt 023 Verification Summary

**Exactly five sequences:** PASS  
**Fixed capacities 12/14/16/14/12:** PASS  
**Chapter overview fully delivered:** PASS  
**Eligible topology-family compatibility:** PASS  
**Sufficient playable payload for each capacity:** PASS  
**Meaningful interaction density:** PASS  
**No sustained flavour-only Player Choice architecture:** PASS  
**No zero-stakes branching sequence:** PASS  
**Chapter encounter budget feasible across 68 entries:** PASS  
**Guaranteed story progress on viable outcomes:** PASS  
**Survival/material gameplay leverage:** PASS  
**Persistent-tag opportunities sparse and payoff-backed:** PASS  
**Handoffs route-safe:** PASS  
**Milestone model preserved:** PASS  
**Sequence-level canon fully adjudicated:** PASS — X approved, X narrowed, X reveal refinements, X rejected  
**FACTS.md updated consistently:** PASS  
**STORY_TAGS.md read-only and unchanged:** PASS  
**No exact clues/evidence/tag mechanics invented:** PASS  
**Ready for detailed sequence planning:** PASS

`PROMPT 023: PASS — READY FOR PROMPT 032`
```

Do not write PASS unless every item genuinely passes.

If a sequence still has:

```text
choice-led / branching
```

while all meaningful survival/material/tag stakes are `None`, PASS is prohibited.

Do not excuse this as:

```text
non-evaluative approach selection
```

or equivalent wording.

---

# 20. Output

Save the verified architecture in place:

```text
[Book]/chapters/chapter-[NN]/sequences.md
```

Update:

```text
[Book]/FACTS.md
```

only for:

- approved/narrowed SCFs;
- verified sequence-level reveal-timing refinements;
- legitimately resolved high-level unresolved matters within this chapter's authority.

Read but do not modify:

```text
[Book]/STORY_TAGS.md
```

Do not create topology selections, sequence plans, mechanics, prose, or publication files.

If `FACTS.md` cannot be safely written:

- do not leave `sequences.md` claiming permanent FACT IDs that were not persisted;
- do not claim PASS;
- report the recovery required.

When complete report briefly:

- verified `sequences.md` path;
- SCF counts by outcome;
- FACT IDs added or reveal timings refined;
- whether any sequence architecture was corrected;
- whether any zero-stakes/choice-heavy sequence was rejected or repaired;
- that `STORY_TAGS.md` was read but not modified;
- whether `PROMPT 023: PASS — READY FOR PROMPT 032` was reached.
