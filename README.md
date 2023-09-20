# DoublePendulum
Double pendulum chaos example. This is a frictionless double pendulum with two lengths and two masses, solved using Lagrangians and the Runge-Kutta method. The tip of length two is tracked with colored circles. Gravity parameters may be changed with the variable g. Friction may be added by setting the dampening constant to some number.
 
Initializes a timer that simulated a double pendulum's motion, updating its positions and refreshes the form.

![load](https://github.com/akingry/DoublePendulum/assets/111338740/7f3eed47-6324-470f-8c84-6b5d46548573)

Implements the Runge-Kutta numerical integration method to update the positions and velocities using four intermediate values (k1, k2, k3, k4) at each time step. It then uses a weighted average of these values to update the pendulum's parameters.

![kutta](https://github.com/akingry/DoublePendulum/assets/111338740/fd770e99-eb13-435c-abe5-fc7d5bdf3b4e)

Calculates the derivatives of the double pendulum's position and velocity with respect to time. The commented-out lines will add damping effects to the pendulum, simulating loss of energy due to friction.

![calc](https://github.com/akingry/DoublePendulum/assets/111338740/0294832b-a1c2-43f1-9e67-e0ec53d217b7)

Visual rendering of a the simulation calculates and draws the positions of the pendulum masses, updates a trail of colored circles at the second pendulum's tip, and removes older circles to maintain a clear representation of the motion.

![paint](https://github.com/akingry/DoublePendulum/assets/111338740/f25cb2d5-2061-4b6e-b0fa-1f5f7aa7a169)
