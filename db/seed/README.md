# Sample order seeds — `K7QM24`

A complete order lifecycle across all three bounded contexts, split into
independently runnable stages so any point in the story can be reproduced.

```powershell
cd db\seed
.\run-all.ps1                # reset + full lifecycle
.\run-all.ps1 -From 04       # replay from the exchange onward
.\run-all.ps1 -ResetOnly     # clear the sample order
```

Each file is a single transaction and prints a one-line result. Run them in
order — every stage assumes the previous one has been applied.

| File | Stage | Leaves the order at |
|---|---|---|
| `00-reset.sql` | remove the sample order and everything derived from it | — |
| `01-order-created.sql` | travellers, itinerary, segments, priced items, time limit | `Pending` |
| `02-payment-settled.sql` | card payment settled and allocated | `Confirmed` |
| `03-delivered.sql` | delivery records issued, invoices accepted | `Delivered` |
| `04-exchange-return-leg.sql` | Sara's return leg moved 9 Sep → 12 Sep | `Delivered` |
| `05-flown.sql` | travel, ancillaries consumed | `Flown` |

## The scenario

**THR → MHD → THR**, two adults, W5-1084 out (2 Sep) and W5-1085 back (9 Sep).

| Item | Type | Traveller | Note |
|---|---|---|---|
| `OI000001` | Flight | Ali | both legs — multi-segment, carries a value allocation |
| `OI000002` | Flight | Sara | both legs — **exchanged in stage 04** |
| `OI000003` | Bag | Ali | associated ancillary |
| `OI000004` | Fee (lounge) | Sara | standalone ancillary |
| `OI000005` | Flight | Sara | replacement created by the exchange |

### Ancillaries and the EMD question

This domain has no EMD type — `agent-build-contract.md` R-19 forbids IATA
vocabulary, and `COMPLETE-DOMAIN-REFERENCE.md` §3.1.3 models the distinction
on `ServiceDelivery` instead:

| Sample data | `AssociatedSegmentUnit` | IATA equivalent |
|---|---|---|
| bag, record `…1000001` unit 3 | `1` | EMD-A (associated) |
| lounge, record `…1000002` unit 3 | `NULL` | EMD-S (standalone) |

An EMD in IATA is a separately numbered financial document. Here a service unit
lives inside the traveller's own delivery record, so no separate document is
raised for it.

## What the exchange demonstrates

Stage 04 is the interesting one. Six rules shape it:

- **I-12 — a price is never mutated.** `OI000002` is not repriced; `OI000005`
  is created with the new price and `ReplacesItem = OI000002`.
- **Journey elements are order-level.** `JE000002` survives the exchange because
  Ali still flies it; only Sara's *segment* is repointed to the new `JE000003`.
- **Confirm before release.** The new segment is confirmed before the old one is
  marked `Rebooked` — reversed, a mid-flow failure leaves the passenger with no seat.
- **`Open → Exchanged` only.** A delivery unit can be exchanged only while `Open`.
  That is why stage 04 runs *before* travel: once `Flown`, the only route is refund.
- **I-32 — a reversal references what it reverses.** A credit note reverses the
  original invoice, then a fresh invoice is issued.
- **Fare difference is a payment, not a price edit.** 2,400,000 IRR on a second record.

The exchanged unit stays `Exchanged` forever and never flies, which is what
drives delivery record 2 to `Closed` rather than `Used` — the mixed-terminal
row 4 of the derived-status table in §3.1.7.

## Known deviations in this data

- **`OperationType.Exchange` is deferred** (reference §5 lists it as deferred, and
  the enum stops at eight members). The exchange operation is recorded with
  `Type = 'Cancellation'` and idempotency key `exchange:K7QM24:TR000002`. When the
  member is enabled, that row should be retyped.
- **Written as SQL, not through domain methods.** Invariants were verified by
  query, not enforced by the aggregates. A test driving `Order.Create` through to
  `Flown` would prove the domain actually produces this shape; tests are paused
  by standing decision (deviation `DV-002`).
