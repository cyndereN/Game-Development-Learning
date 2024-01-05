
https://www.youtube.com/watch?v=fdAOPHgW7qM&ab_channel=JonathanBlow

- ballistic motion

    如果Physical Simulation的时间间隔过长

    向前判断什么时候发生collision，然后将时间分割成两段

    Sim(t0....t1/2), Sim(t1/2...t1)

    engine code impacting

    ```lua
    loop:
        read_input();
        simulation();
        render();

    simulation(){
        for all_entities update();
        do_ballistic_motion();
    }
    ```

- ad-hoc motion

    如果have more things happening than physical obj collision，需要order correctly

    Tunneling

    gameplay code impacting

- game-driven state changes

    e.g. crate a monster if after a certain period of time if cannon updates

    ```cpp
    Cannon::Update(dt){ // 'dt' is a per-frame global
        countdown -= dt;  // countdown usually goes negative
        while(countdown <= 0 ){        // instead of if <=0, reset countdown, we do this
            countdown += time_per_shot; // += to make compensation
            m = create_monster();
            sim_monster(-countdown); // "rewind" to make monster at correct state
            play_shot_soon();
        }
    }
    ```

    entities update in arbitrary order. All update() procedures must conform to this scheme

    ```cpp
    Cannon::Tell_me_the_first_time_you_would_do_sth_and_what_that_would_be(){
        return time, event_info;
    }

    Cannon::Okay_do_that_thing_you_said(time, event_info){
        switch event_info.type{
            ...
        }
    }

    Cannon::maybe_do_earliest_thing(bool for_real){
        if(for_real) do
        else return time?
    }
    ```

- read_input()

    How often do you read player input?

    And when do you consider it as having occurred?

    When running recorded input from a TAS, produce the same results always

    Read input as often as possible, have frame rate as high and consistent as possible

    High physics update rate:

    - Nothing conspicuously wrong
    - Can be made consistent with fixed timestep (e.g. 1/60 sec)


- Problem with "frame-rate independent"

    Always interpolate? Draw interpolate(Sn, Sn+1, l)
    
    bring latency

- Update()和FixUpdate()

    https://blog.csdn.net/Marine_snow/article/details/117127889