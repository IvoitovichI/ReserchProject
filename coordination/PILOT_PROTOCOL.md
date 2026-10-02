# DungeonTrace Pilot Protocol v1.0

Status: User-provided protocol baseline. This document does not itself demonstrate KBTU/supervisor or ethics approval; see `BLK-004` before recruitment or real data collection.

## Identity, consent and scope

- Protocol ID: `PILOT-PROTOCOL-v1.0`.
- Consent ID/date: `PILOT-CONSENT-v1.0`, `2026-09-30`.
- Eligible participant: adult (18+) who confirms voluntary participation and all consent checkboxes.
- The client records only the pseudonymous `participant_id`, assigned `session_id`, `run_id`, `condition_id`, build/schema/content/protocol/consent versions, seed, gameplay events, controlled technical/performance values, and approved questionnaire records in their separate protected store.
- It must not collect name, email, telephone, IP address, precise hardware identifier, audio, video, screen recording, precise location, medical information, or participant free text.

### Participant-facing consent text

> **Согласие на участие в исследовании**
>
> Вы приглашены принять участие в пилотном исследовании системы анализа поведения игроков. Цель исследования — проверить, можно ли использовать игровую телеметрию для определения поведенческих и мотивационных особенностей игроков.
>
> Во время исследования вам будет предложено заполнить краткую анкету и сыграть в FP3D-игру. Ожидаемая продолжительность участия составляет приблизительно 20–30 минут.
>
> Игра будет автоматически записывать игровые события: перемещение по комнатам, продолжительность прохождения, использование оружия, получение урона, смерти, повторные попытки, нахождение секретов, получение и расходование игровой валюты и выбор улучшений.
>
> Исследование не собирает ваше имя, email, аудио, видео, пароли или точный системный идентификатор устройства. Ваши данные будут связаны только с псевдонимным кодом участника.
>
> Возможные неудобства включают усталость, напряжение глаз, головную боль, головокружение, тошноту или дискомфорт от игры от первого лица. Вы можете сделать перерыв или прекратить участие в любой момент без объяснения причины и каких-либо негативных последствий.
>
> Участие является добровольным. Вы можете отказаться от участия до начала исследования или запросить удаление своих данных в соответствии с правилами withdrawal.
>
> Нажимая кнопку «Я согласен(-на) участвовать», вы подтверждаете, что ознакомились с этой информацией, понимаете условия исследования и добровольно соглашаетесь принять участие.

Required checkboxes: read study description; understand collected gameplay data; understand right to stop; are 18+; voluntary participation. The start button remains disabled until all are affirmed.

`consent_recorded` uses the v1 envelope timestamp as `consent_timestamp_utc`, `consent_version: PILOT-CONSENT-v1.0`, and `consent_accepted`. A material change to purpose, data, retention, or withdrawal requires a new consent version; it never retroactively replaces prior consent.

## Storage, access and withdrawal

- Pseudonymised pilot data are retained for at most three years after study completion/publication, subject to the stricter final KBTU rule.
- Raw JSONL/CSV stay in a protected directory; backups are encrypted; no raw data goes to public Git repositories or public AI services. Any identity↔participant-code lookup is separate.
- Raw pseudonymised data access is limited to the principal researcher, supervisor, and approved research-group members with an analysis need. Developers/designers receive aggregates, de-identified metrics and issue reports by default. Each access transfer records recipient, dataset, purpose, date, duration and closure/result.
- A participant may refuse before play, stop at any time, skip an individual question, or request deletion using `participant_id` within 14 calendar days or before irreversible aggregation. The withdrawal reason is optional; do not collect it as free text or diagnosis.
- On stop, telemetry stops immediately. The participant may separately permit retaining already-collected de-identified partial data; otherwise exclude it from the primary analysis. A deletion request/action is recorded neutrally. Individual deletion may become impossible after irreversible anonymous aggregation; disclose this before consent.

## Comfort, safety and technical stop

- Offer a break at 20 minutes; no uninterrupted session exceeds 30 minutes. Sensitivity, volume and supported accessibility settings remain adjustable. No automatic resumption after stop; stopping cannot affect reward, assessment or university relationship.
- Stop immediately for a participant request, motion sickness, eye strain, headache, anxiety/stress, physical discomfort, inability to continue comfortably, loss of orientation, or inability to confirm voluntary consent. Pause/stop telemetry, offer a break/water/rest, never persuade continuation, and resume only on participant initiative after discomfort has fully resolved. If symptoms persist/worsen, end and seek appropriate assistance.
- Record only the controlled neutral category: `participant_request`, `motion_sickness`, `eye_strain`, `headache`, `anxiety_or_stress`, `physical_discomfort`, `technical_failure`, `researcher_decision`, or `other_or_not_provided` — never a medical diagnosis.
- Mark `technical_exclusion` and keep it separately for technical QA (unless deletion is requested) when consent/version, participant/condition ID, event time/order, condition assignment, PII safety, build validity, or essential data integrity fails.

## Pre-specified player-study design

- Game condition: `GAME_STANDARD_V1`; same fixed difficulty/content, no adaptive intervention, two short runs per participant, validated `SEED_A` and `SEED_B`, order counterbalanced `AB`/`BA` and, where feasible, stratified by low/medium/high FPS-roguelite experience.
- Offline paired analytic comparison for every participant: `A0_BASELINE` uses non-sequential aggregates; `A1_SDK_SEQUENCE` uses ordered events, segmentation, gameplay context and continuous profiles. Questionnaire results are unavailable to the SDK during profiling and serve only as later criterion measures.
- Primary accuracy outcome: **Macro Motivation Profile Convergence** — Fisher-z mean of the four Spearman correlations between SDK and Hexad scores for Achievement, Free Spirit/Exploration, Player/Reward Orientation and Disruptor/System Testing. Secondary outcomes: per-axis correlations, 1–7 scaled MAE, Top-2 overlap, cross-run stability, A0↔A1 difference, and error associations with experience/performance.
- Bartle is a secondary convergent measure: compare Achiever and Explorer; keep Socializer exploratory; do not equate Killer with Disruptor.
- Prefer Hexad/Bartle 24–72 hours before play, with counterbalanced instrument order and item order. If that is impractical, collect after gameplay before hypothesis disclosure. Use only protocol-approved instrument wording/licensing and store raw answers outside telemetry.

## Pre-specified designer evaluation

- Blind labels `Report A`/`Report B`: `D0_METRICS_ONLY` receives conventional metric tables; `D1_SDK_CARD` receives profile, confidence, telemetry evidence and design recommendations. Use counterbalanced incomplete blocks: no rater sees the same case twice; each case is evaluated under both conditions across the sample.
- Rate clarity, evidence traceability, profile plausibility, actionability, specificity, contextual relevance, prioritisation, uncertainty calibration, player agency/safety and overall usefulness on 1–5 anchors. The primary interpretability/actionability subscore is the mean of clarity, traceability, actionability and contextual relevance; compare `D1-D0` separately from accuracy.
- Pilot feasibility targets: `D1` mean at least 4.0/5, no primary rubric criterion mean below 3.0, and at least +0.5 versus `D0`; formal main-study thresholds/tests require approval before analysis.
- The proposed open-ended designer answer is **not** in gameplay telemetry. It may be used only as a separately consented/approved evaluator artifact with PII screening and controlled access; otherwise replace it with the listed coded categories.
