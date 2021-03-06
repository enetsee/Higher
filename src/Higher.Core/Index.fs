﻿namespace Higher.Core

// Index-preserving Function
type Index<'F, 'G> =
    abstract Invoke<'A> : App<'F, 'A> -> App<'G, 'A>

type Index private () =
    static let token = new Index()
    static member Inj (value : Index<'F, 'G>) : App2<Index, 'F, 'G> =
        let app = new App<Index, 'F>(token, value)
        new App2<Index, 'F, 'G>(AppToken<Index, 'F>.Token token, app)
    static member Prj (app2 : App2<Index, 'F, 'G>) : Index<'F, 'G> = 
        let app = app2.Apply(AppToken<Index, 'F>.Token token) :?> App<Index, 'F>
        app.Apply(token) :?> _
    static member Run(index : App2<Index, 'F, 'G>) = 
        Index.Prj index


// Index-preserving Category instance
type IndexCategory() = 
    inherit Category<Index>() with
    override self.Ident<'F>() = 
        Index.Inj { new Index<'F, 'F> with 
                        member self.Invoke x = x }
    override self.Compose f g = 
        Index.Inj ({ new Index<'A, 'C> with 
                        member self.Invoke x = 
                            (Index.Prj g).Invoke <| (Index.Prj f).Invoke x })