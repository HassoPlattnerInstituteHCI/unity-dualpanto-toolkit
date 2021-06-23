The PantoIntroducer (initially used to introduce the layout of levels to players) allows you to design Panto-movements and speech-outputs right in the Unity Editor. If there was an equivalent to cut-scenes, this would be it.

# How to use
To quickly get started, check out the scene `Introductions/SampleIntroductions.unity` and the prefabs in `Introductions/Prefabs/`.
To make an introduction (= a sequence of operations to be run one by one), you need a PantoIntroducer. This runs all sub-intros one after another when a specified button is pressed or RunIntros() is called (awaitable).
You can call CancelIntros() or hit a specified key (X by default) to cancel a running introduction of a PantoIntroducer.
The actual content of the introduction is most conveniently specified by adding child-objects inheriting from PantoIntroBase to the Introducer. You can easily change their order or disable certain parts.

Some sub-intros come with slightly hacky visualizations. In-game they are active when the intro is being run. In the editor you can disable or enable them via a button in the inspector while you have the Introducer selected. 

The available intros are specified below, though you can implement your own as well.
It is recommended to use the prefabs to get visualizations and otherwise necessary components. However, the scaling could get weird depending on how the Panto-object is scaled. You may want to adjust it in that case and just copy-paste new intros from there.


# Sub-Intros

## PantoIntroSpeaker
Plays an audio clip or says a text. You only need to specify one. If both are given, the clip will be used. This also goes for any audio specified in other Sub-Intros.

## PantoIntroPointer
Moves the handle to the location of this gameobject. You can specify audio while on the way and when reaching the point.

## PantoIntroTracer
Moves the handle along a path. You can specify audio on the way to each vertex or while stopping at a vertex.
To edit the path, you need to edit the connected LineRenderer. Unity has two buttons for that in the inspector. Activate the first to move existing vertices. Use box selection to select them and move them with the Move Tool as usual. Activate the second button to add new vertices by clicking. Note that LineRenderers are actually 3-dimensional, so things can look wonky, especially when rotating the viewport. To easily flatten the path, use the button on the parent Introducer.

## PantoIntroBackAndForth
Moves the handle back and forth on a path until an audio/text was played/spoken or a number of rounds has passed.
For editing the path, see PantoIntroTracer above.

## PantoIntroPause
Waits for a certain time. That's it.

## PantoParallelIntros
This is a meta-introduction. Use it if you want to run multiple sub-Intros simultaneously. Simply parent them to this to run them simultaneously and proceed once all of them are finished.
