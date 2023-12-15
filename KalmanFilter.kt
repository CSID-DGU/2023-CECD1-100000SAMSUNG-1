package com.bharathvishal.messagecommunicationusingwearabledatalayer

//class KalmanFilter(initValue: Double) {
//
//    private var Q: Double = 0.00001//프로세스 노이즈
//    private var R: Double = 0.001//측정 잡음 공분산 행렬값
//    private var X: Double = initValue
//    private var P: Double = 1.0//추정 오차 공분산 행렬값
//    private var K: Double = 0.0//칼만이득
//
//    init {
//        X = initValue
//    }
//
//    private fun measurementUpdate() {
//        K = (P + Q) / (P + Q + R)
//        P = R * (P + Q) / (R + P + Q)
//    }
//
//    fun update(measurement: Double): Double {
//        measurementUpdate()
//        X = X + (measurement - X) * K
//        return X
//    }
//
//    fun stop() {
//        K = 0.0
//        P = 1.0
//        X = 0.0
//    }
//}

class KalmanFilter(q: Float,r: Float,p: Float,  x: Float, k: Float) {

    private var q: Float = q // process noise covariance
    private var r: Float = r // measurement noise covariance
    private var x: Float = x // estimated value
    private var p: Float = p // estimation error covariance
    private var k: Float = k // kalman gain

    fun update(z: Float): Float {
        // prediction update
        p = p + q

        // measurement update
        k = p / (p + r)
        x = x + k * (z - x)
        p = (1 - k) * p

        return x
    }
}
