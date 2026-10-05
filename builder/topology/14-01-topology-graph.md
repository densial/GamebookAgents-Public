# 14-01 Topology Graph

```text
                         01
                       /    \
                     02      03
                      |       |
                     04      05
                    /  \    /  \
                  06   07  08   09
                   \   /    \   /
                    10       11
                     |        |
                    12       13
                      \      /
                        14
                         |
                       EXIT
```

The two primary routes split at Node 01 and remain separate until the global reconvergence at Node 14.

- Nodes 04 and 05 provide branch-local two-outcome resolution points.
- Nodes 06/07 reconverge at Node 10.
- Nodes 08/09 reconverge at Node 11.
- Nodes 12 and 13 provide branch-specific development after those local reconvergences.
- Node 14 is the first shared node across both primary routes and provides the common development/handoff.

The exact authoritative connections and play-experience constraints are defined in `14-01-topology-plan.md`.
