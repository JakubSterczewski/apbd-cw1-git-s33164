Krótko opisz w README, dlaczego tym razem merge nie był fast-forward.
- Historie obu gałęzi się rozeszły, więc git musiał utworzyć commit merge, aby połączyć obie linie historii.

• 1. Kiedy Git wykona fast-forward, a kiedy powstaje merge commit?
- Fast-forward występuje, kiedy nie ma nowych commitów na docelowej gałęzi.
- Natomiast merge commit powstaje w przypadku, gdy na docelowej gałęzi wystąpiły nowe commity lub w przypadku --no-ff. W takich przypadkach tworzony jest nowy merge commit łączący dwie gałęzie.

• 2. Czym w praktyce różni się merge od rebase?
- Merge łączy dwie gałęzie w jedną, natomiast rebase zmienia bazowy stan, z którego powstał feature, tworząc liniową historię. 

• 3. W jaki sposób został rozwiązany konflikt w Twoim repozytorium?
- Konfilkt wystepował w wypisywaniu logu exit. Został on rozwiązany ręcznie.