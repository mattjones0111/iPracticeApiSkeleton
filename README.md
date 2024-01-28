# iPractice Technical Assignment - Solution

Hi!

I'm really pleased to submit my solution to the technical assignment.

The project is arranged into vertical slices which is all tied together by MediatR. MediatR is
a simple library that provides a low-ceremony implementation of the Command pattern.

Full disclosure - I've used this pattern elsewhere (particuarly in a side-project of mine) so
alot of what I have done here is inspired by previous work I have done.

Functionality exists as 'Features' (see the Domain project, Features folder). Each feature is a
self-contained file, and has affinity with a single HTTP request invoked via the Controllers in
the Api project.

The 'vertical sliced' feature files contain EVERYTHING relevant to the given request, from
the structure of the request itself, to how it is validated to how it is handled. This means that
a reviewer has all the code they need in a single place in order to diagnose/fix any issues.

## My thoughts

There is a good chance this is over-engineered. I have deliberately done so on the assumption
that this service will eventually have more complexity than a simple appointment booking application
in the future. The point is to show that I have facilitated adding further complexity is managed 
by allowing new self-contained features to be added, rather than modifying existing service/repository
types.

Another reason for this architecture are that it provides a better abstraction over responsibilities
than the typical Controller -> Service -> Repository -> Data pattern, and provides a much easier and cleaner
way to cut in aspects (cross-cutting concerns).

Testing is deliberately done by default ONLY at integration level. I have some strong opinions about test
driven development, and I prefer fewer, broader integration-level tests than multitudes of smaller
"unit" tests. The reason for this is that I have seen many codebases where there are so many highly-mocked
tests that exercise implementation rather than behavior. Very often there are so many tests that
refactoring is much harder as a result.

Finally, sorry for not fully completing the "modify availability" flow - this was simply because of
time pressures this weekend.

In either case, I would be delighted to get your feedback and to discuss this further.

Matt Jones
28th Jan 2024
