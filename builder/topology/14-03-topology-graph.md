# 14-03 Topology Graph

```mermaid
flowchart TD
    N01["01"] --> N02["02"]
    N01 --> N03["03"]

    N02 --> N04["04"]
    N02 --> N05["05"]
    N03 --> N05
    N03 --> N06["06"]

    N04 --> N07["07"]
    N05 --> N07
    N06 --> N07

    N07 --> N08["08"]
    N07 --> N09["09"]

    N08 --> N10["10"]
    N08 --> N11["11"]
    N09 --> N11
    N09 --> N12["12"]

    N10 --> N13["13"]
    N11 --> N13
    N12 --> N13

    N13 --> N14["14"]
    N14 --> EXIT["EXIT"]
```

## Exact structure

```text
FIRST PHASE

01 → 02 / 03

02 → 04 / 05
03 → 05 / 06

04 → 07
05 → 07
06 → 07


SECOND PHASE

07 → 08 / 09

08 → 10 / 11
09 → 11 / 12

10 → 13
11 → 13
12 → 13

13 → 14
14 → EXIT
```

## Topology identity

`14-03` is the **double-diamond / midpoint-reset topology**.

- Node 01 begins the first meaningful approach split.
- Nodes 02 and 03 resolve the first-phase approaches.
- Nodes 04, 05, and 06 pay off those outcomes.
- Node 07 is a genuine shared midpoint and establishes new common progression.
- Node 07 then launches a second independent split.
- Nodes 08 and 09 resolve the second-phase approaches.
- Nodes 10, 11, and 12 pay off those outcomes.
- Node 13 is the final global reconvergence.
- Node 14 provides shared development/handoff before exit.
- Every complete continuing route contains eight entries.

The exact authoritative connections and play-experience constraints are defined in `14-03-topology-plan.md`.
