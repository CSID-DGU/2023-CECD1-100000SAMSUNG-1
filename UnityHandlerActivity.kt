package com.bharathvishal.messagecommunicationusingwearabledatalayer


import android.annotation.SuppressLint
import android.app.Activity
import android.content.Context
import android.net.Uri
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Button
import android.widget.FrameLayout
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.bharathvishal.messagecommunicationusingwearabledatalayer.databinding.UnityhandleractivityBinding
import com.google.android.gms.tasks.Tasks
import com.google.android.gms.wearable.CapabilityClient
import com.google.android.gms.wearable.CapabilityInfo
import com.google.android.gms.wearable.DataClient
import com.google.android.gms.wearable.DataEventBuffer
import com.google.android.gms.wearable.MessageClient
import com.google.android.gms.wearable.MessageEvent
import com.google.android.gms.wearable.Wearable
import com.unity3d.player.UnityPlayer
import kotlinx.coroutines.*
import java.nio.charset.StandardCharsets


class UnityHandlerActivity : AppCompatActivity(), CoroutineScope by MainScope(),
    DataClient.OnDataChangedListener,
    MessageClient.OnMessageReceivedListener,
    CapabilityClient.OnCapabilityChangedListener{
    var activityContext: Context? = null
    private val wearableAppCheckPayload = "AppOpenWearable"
    private val wearableAppCheckPayloadReturnACK = "AppOpenWearableACK"
    private var wearableDeviceConnected: Boolean = false

    private var currentAckFromWearForAppOpenCheck: String? = null
    private val APP_OPEN_WEARABLE_PAYLOAD_PATH = "/APP_OPEN_WEARABLE_PAYLOAD"

    private val MESSAGE_ITEM_RECEIVED_PATH: String = "/message-item-received"

    private val TAG_GET_NODES: String = "getnodes1"
    private val TAG_MESSAGE_RECEIVED: String = "receive1"

    private var messageEvent: MessageEvent? = null
    private var wearableNodeUri: String? = null

    private lateinit var binding: UnityhandleractivityBinding

    private lateinit var thread : Thread

    //이동거리, 속도, 가속도 값

    private var beforeAccX = 0.0f
    private var beforeAccY = 0.0f
    private var beforeAccZ = 0.0f
    private var beforeVeloX = 0.0f
    private var beforeVeloY= 0.0f
    private var beforeVeloZ = 0.0f
    private var beforeDistX = 0.0f
    private var beforeDistY= 0.0f
    private var beforeDistZ= 0.0f


    //필터링된, 이동거리, 속도, 가속도 값
    private var beforeFAccX = 0.0f
    private var beforeFAccY = 0.0f
    private var beforeFAccZ = 0.0f
    private var beforeFVeloX = 0.0f
    private var beforeFVeloY= 0.0f
    private var beforeFVeloZ = 0.0f
    private var beforeFDistX = 0.0f
    private var beforeFDistY= 0.0f
    private var beforeFDistZ= 0.0f


    private var startTime : Long = 0
    private var lastTimestamp = System.currentTimeMillis()

    private lateinit var  buttonClick : Button
    private lateinit var checkWearableBtn :Button
    private lateinit var deviceconnectionStatusTv:TextView
    private lateinit var unityPlayer: UnityPlayer
    private lateinit var messagelogTextView: TextView

    private var count = 0f
    private var move = 0f

    private var sumX = 0.0f
    private var sumY = 0.0f
    private var sumZ = 0.0f

    private var sumAccX = 0.0f
    private var sumAccY = 0.0f
    private var sumAccZ = 0.0f

    private var KVX = 0.0f
    private var KVY = 0.0f
    private var KVZ = 0.0f

    private var KDX = 0.0f
    private var KDY = 0.0f
    private var KDZ = 0.0f

    private var KalmanAccX : KalmanFilter? = null
    private var KalmanAccY : KalmanFilter? = null
    private var KalmanAccZ : KalmanFilter? = null

    private var KalmanVeloX: KalmanFilter? = null
    private var KalmanVeloY: KalmanFilter? = null
    private var KalmanVeloZ: KalmanFilter? = null

    private var KalmanDistX: KalmanFilter? = null
    private var KalmanDistY: KalmanFilter? = null
    private var KalmanDistZ: KalmanFilter? = null

    private var mPosX = 0f // 공의 초기 X 위치

    private var mPosY = 0f // 공의 초기 Y 위치

    private var mPosZ = 0f // 공의 초기 Y 위치

    private var mVelX = 0f // 공의 초기 X 방향 속도

    private var mVelY = 0f

    private var mVelZ = 0f

    private var maxVelo = 0f

    @SuppressLint("MissingInflatedId")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        binding = UnityhandleractivityBinding.inflate(layoutInflater)
        val view = binding.root
        setContentView(R.layout.unityhandleractivity)

        buttonClick = findViewById(R.id.buttonClick)
        checkWearableBtn = findViewById(R.id.checkwearablesButton)
        deviceconnectionStatusTv = findViewById(R.id.deviceconnectionStatusTv)
        messagelogTextView = findViewById(R.id.messagelogTextView)

        unityPlayer = UnityPlayer(this)
        val unityContainer: FrameLayout = findViewById(R.id.unityContainer)
        unityContainer.addView(unityPlayer)

        unityPlayer.windowFocusChanged(true)
        unityPlayer.requestFocus()

        activityContext = this
        wearableDeviceConnected = false
        checkWearableBtn = findViewById(R.id.checkwearablesButton)
        try{
            checkWearableBtn.setOnClickListener {
                if (!wearableDeviceConnected) {
                    val tempAct: Activity = activityContext as UnityHandlerActivity
                    //Couroutine
                    initialiseDevicePairing(tempAct)
                    Log.d("checkWear","false")
                }
                Log.d("checkWear","true")
            }
        }catch (e:Exception){
            Log.e("checkWear","$e")
        }

//칼만필터 초기화
        KalmanAccX = KalmanFilter(0.00009f,0.0003f,1.0f,0.0f,0.0f)
        KalmanAccY = KalmanFilter(0.00009f,0.0003f,1.0f,0.0f,0.0f)
        KalmanAccZ = KalmanFilter(0.00009f,0.00008f,1.0f,0.0f,0.0f)

        KalmanVeloX = KalmanFilter(0.00001f,0.001f,1.0f,0.0f,0.0f);
        KalmanVeloY = KalmanFilter(0.00001f,0.001f,1.0f,0.0f,0.0f);
        KalmanVeloZ = KalmanFilter(0.00001f,0.001f,1.0f,0.0f,0.0f);

        KalmanDistX = KalmanFilter(0.00001f,0.001f,1.0f,0.0f,0.0f);
        KalmanDistY = KalmanFilter(0.00001f,0.001f,1.0f,0.0f,0.0f);
        KalmanDistZ = KalmanFilter(0.00001f,0.001f,1.0f,0.0f,0.0f);


///////////////메시지 보내는 메소드
//        binding.sendmessageButton.setOnClickListener {
//            if (wearableDeviceConnected) {
//                if (binding.messagecontentEditText.text!!.isNotEmpty()) {
//
//                    val nodeId: String = messageEvent?.sourceNodeId!!
//                    // Set the data of the message to be the bytes of the Uri.
//                    val payload: ByteArray =
//                        binding.messagecontentEditText.text.toString().toByteArray()
//
//                    // Send the rpc
//                    // Instantiates clients without member variables, as clients are inexpensive to
//                    // create. (They are cached and shared between GoogleApi instances.)
//                    val sendMessageTask =
//                        Wearable.getMessageClient(activityContext!!)
//                            .sendMessage(nodeId, MESSAGE_ITEM_RECEIVED_PATH, payload)
//
//                    sendMessageTask.addOnCompleteListener {
//                        if (it.isSuccessful) {
//                            Log.d("send1", "Message sent successfully")
//                            val sbTemp = StringBuilder()
//                            sbTemp.append("\n")
//                            sbTemp.append(binding.messagecontentEditText.text.toString())
//                            sbTemp.append(" (Sent to Wearable)")
//                            Log.d("receive1", " $sbTemp")
//                            binding.messagelogTextView.append(sbTemp)
//
//                            binding.scrollviewText.requestFocus()
//                            binding.scrollviewText.post {
//                                binding.scrollviewText.scrollTo(0, binding.scrollviewText.bottom)
//                            }
//                        } else {
//                            Log.d("send1", "Message failed.")
//                        }
//                    }
//                } else {
//                    Toast.makeText(
//                        activityContext,
//                        "Message content is empty. Please enter some message and proceed",
//                        Toast.LENGTH_SHORT
//                    ).show()
//                }
//            }
//        }

    }

    override fun onPause() {
        super.onPause()
        unityPlayer.pause()

        try {
            Wearable.getDataClient(activityContext!!).removeListener(this)
            Wearable.getMessageClient(activityContext!!).removeListener(this)
            Wearable.getCapabilityClient(activityContext!!).removeListener(this)
        } catch (e: Exception) {
            e.printStackTrace()
        }
    }

    override fun onResume() {
        super.onResume()
        unityPlayer.resume()

        buttonClick.setOnClickListener(){
            UnityPlayer.UnitySendMessage("Manager","endExcercise","")
        }

        try {
            Wearable.getDataClient(activityContext!!).addListener(this)
            Wearable.getMessageClient(activityContext!!).addListener(this)
            Wearable.getCapabilityClient(activityContext!!)
                .addListener(this, Uri.parse("wear://"), CapabilityClient.FILTER_REACHABLE)
        } catch (e: Exception) {
            e.printStackTrace()
        }

    }

    override fun onDestroy() {
        unityPlayer.quit()
        super.onDestroy()
    }

    @SuppressLint("SetTextI18n")
    private fun initialiseDevicePairing(tempAct: Activity) {
        //Coroutine
        launch(Dispatchers.Default) {
            var getNodesResBool: BooleanArray? = null

            try {
                getNodesResBool =
                    getNodes(tempAct.applicationContext)
            } catch (e: Exception) {
                e.printStackTrace()
            }

            //UI Thread
            withContext(Dispatchers.Main) {
                if (getNodesResBool!![0]) {
                    //if message Acknowlegement Received
                    if (getNodesResBool[1]) {
                        Toast.makeText(
                            activityContext,
                            "Wearable device paired and app is open. Tap the \"Send Message to Wearable\" button to send the message to your wearable device.",
                            Toast.LENGTH_LONG
                        ).show()
                        deviceconnectionStatusTv.text =
                            "Wearable device paired and app is open."
                        deviceconnectionStatusTv.visibility = View.VISIBLE
                        wearableDeviceConnected = true
                        //binding.sendmessageButton.visibility = View.VISIBLE
                    } else {
                        Toast.makeText(
                            activityContext,
                            "A wearable device is paired but the wearable app on your watch isn't open. Launch the wearable app and try again.",
                            Toast.LENGTH_LONG
                        ).show()
                        deviceconnectionStatusTv.text =
                            "Wearable device paired but app isn't open."
                        deviceconnectionStatusTv.visibility = View.VISIBLE
                        wearableDeviceConnected = false
                        //binding.sendmessageButton.visibility = View.GONE
                    }
                } else {
                    Toast.makeText(
                        activityContext,
                        "No wearable device paired. Pair a wearable device to your phone using the Wear OS app and try again.",
                        Toast.LENGTH_LONG
                    ).show()
                    deviceconnectionStatusTv.text =
                        "Wearable device not paired and connected."
                    deviceconnectionStatusTv.visibility = View.VISIBLE
                    wearableDeviceConnected = false
                    //binding.sendmessageButton.visibility = View.GONE
                }
            }
        }
    }


    private fun getNodes(context: Context): BooleanArray {
        val nodeResults = HashSet<String>()
        val resBool = BooleanArray(2)
        resBool[0] = false //nodePresent
        resBool[1] = false //wearableReturnAckReceived
        val nodeListTask =
            Wearable.getNodeClient(context).connectedNodes
        try {
            // Block on a task and get the result synchronously (because this is on a background thread).
            val nodes =
                Tasks.await(
                    nodeListTask
                )
            Log.e(TAG_GET_NODES, "Task fetched nodes")
            for (node in nodes) {
                Log.e(TAG_GET_NODES, "inside loop")
                nodeResults.add(node.id)
                try {
                    val nodeId = node.id
                    // Set the data of the message to be the bytes of the Uri.
                    val payload: ByteArray = wearableAppCheckPayload.toByteArray()
                    // Send the rpc
                    // Instantiates clients without member variables, as clients are inexpensive to
                    // create. (They are cached and shared between GoogleApi instances.)
                    val sendMessageTask =
                        Wearable.getMessageClient(context)
                            .sendMessage(nodeId, APP_OPEN_WEARABLE_PAYLOAD_PATH, payload)
                    try {
                        // Block on a task and get the result synchronously (because this is on a background thread).
                        val result = Tasks.await(sendMessageTask)
                        Log.d(TAG_GET_NODES, "send message result : $result")
                        resBool[0] = true

                        //Wait for 700 ms/0.7 sec for the acknowledgement message
                        //Wait 1
                        if (currentAckFromWearForAppOpenCheck != wearableAppCheckPayloadReturnACK) {
                            Thread.sleep(100)
                            Log.d(TAG_GET_NODES, "ACK thread sleep 1")
                        }
                        if (currentAckFromWearForAppOpenCheck == wearableAppCheckPayloadReturnACK) {
                            resBool[1] = true
                            return resBool
                        }
                        //Wait 2
                        if (currentAckFromWearForAppOpenCheck != wearableAppCheckPayloadReturnACK) {
                            Thread.sleep(250)
                            Log.d(TAG_GET_NODES, "ACK thread sleep 2")
                        }
                        if (currentAckFromWearForAppOpenCheck == wearableAppCheckPayloadReturnACK) {
                            resBool[1] = true
                            return resBool
                        }
                        //Wait 3
                        if (currentAckFromWearForAppOpenCheck != wearableAppCheckPayloadReturnACK) {
                            Thread.sleep(350)
                            Log.d(TAG_GET_NODES, "ACK thread sleep 5")
                        }
                        if (currentAckFromWearForAppOpenCheck == wearableAppCheckPayloadReturnACK) {
                            resBool[1] = true
                            return resBool
                        }
                        resBool[1] = false
                        Log.d(
                            TAG_GET_NODES,
                            "ACK thread timeout, no message received from the wearable "
                        )
                    } catch (exception: Exception) {
                        exception.printStackTrace()
                    }
                } catch (e1: Exception) {
                    Log.d(TAG_GET_NODES, "send message exception")
                    e1.printStackTrace()
                }
            } //end of for loop
        } catch (exception: Exception) {
            Log.e(TAG_GET_NODES, "Task failed: $exception")
            exception.printStackTrace()
        }
        return resBool
    }

    @SuppressLint("SetTextI18n")
    override fun onMessageReceived(p0: MessageEvent) {
        try {
            val s =
                String(p0.data, StandardCharsets.UTF_8)
            val messageEventPath: String = p0.path
            Log.d(
                TAG_MESSAGE_RECEIVED,
                "onMessageReceived() Received a message from watch:"
                        + p0.requestId
                        + " "
                        + messageEventPath
                        + " "
                        + s
            )
            if (messageEventPath == APP_OPEN_WEARABLE_PAYLOAD_PATH) {
                currentAckFromWearForAppOpenCheck = s
                Log.d(
                    TAG_MESSAGE_RECEIVED,
                    "Received acknowledgement message that app is open in wear"
                )

                val sbTemp = StringBuilder()
                sbTemp.append(messagelogTextView.text.toString())
                sbTemp.append("\nWearable device connected1.")
                Log.d("receive1", " $sbTemp")
                messagelogTextView.text = sbTemp
                //binding.textInputLayout.visibility = View.VISIBLE

                checkWearableBtn.visibility = View.GONE
                messageEvent = p0
                wearableNodeUri = p0.sourceNodeId
            } else if (messageEventPath.isNotEmpty() && messageEventPath == MESSAGE_ITEM_RECEIVED_PATH) {

                try {
                    messagelogTextView.visibility = View.VISIBLE
                   // binding.textInputLayout.visibility = View.VISIBLE
                    //binding.sendmessageButton.visibility = View.VISIBLE

                    //시간 측정 함수
                    if(startTime == 0L){
                        //첫 데이터 받을때 설정
                        startTime = System.currentTimeMillis()
                    }
                    //데이터를 받은 시간 설정
                    val currentTime = System.currentTimeMillis()
                    val elapsedTime = (currentTime - startTime)
                    val deltaTime = (currentTime - lastTimestamp) / 1000.0f



                    lifecycleScope.launch(Dispatchers.IO) {
                        //가속도데이터 xyz 분류
                        var s_arr : List<String> = s.split("\\s".toRegex())
                        Log.d("1",s_arr.toString())
                        var charX = s_arr[0]
                        var charY = s_arr[1]
                        var charZ = s_arr[2]

                        var rawAccX = charX.toFloat()
                        var rawAccY = charY.toFloat()
                        var rawAccZ = charZ.toFloat()

                        sumX = sumX + charX.toFloat()
                        sumY = sumY + charY.toFloat()
                        sumZ = sumZ + charZ.toFloat()
                        count = count + 1
                        Log.d("AVG","X : ${sumX/count}, Y : ${sumY/count}, Z : ${sumZ/count}")

                        var accX = rawAccX
                        var accY = rawAccY
                        var accZ = rawAccZ
                        //평균 노이즈 제거
                        accX -= sumX/count
                        accY -= sumY/count
                        accZ -= sumZ/count

                         accX = KalmanAccX!!.update(accX.toFloat())
                         accY = KalmanAccY!!.update(accY.toFloat())
                         accZ = KalmanAccZ!!.update(accZ.toFloat())

                        sumAccX += accX
                        sumAccY += accY
                        sumAccZ += accZ



                        val five_pointAccX = String.format("%.5f",accX)
                        val five_pointAccY = String.format("%.5f",accY)
                        val five_pointAccZ = String.format("%.5f",accZ)

                        val five_pointVelX = String.format("%.5f",mVelX)
                        val five_pointVelY = String.format("%.5f",mVelY)
                        val five_pointVelZ = String.format("%.5f",mVelZ)

                        val five_pointDisX = String.format("%.5f",mPosX)
                        val five_pointDisY = String.format("%.5f",mPosY)
                        val five_pointDisZ = String.format("%.5f",mPosZ)

                        UnityPlayer.UnitySendMessage("Manager","calculateDis","${deltaTime},${elapsedTime/1000.0},${accZ},${accX},${accY}")

                        val sbTemp = StringBuilder()
                        sbTemp.append("\n")
                        sbTemp.append(s)
                        sbTemp.append(" - (Received from wearable)")
                        Log.d("receive1", " $sbTemp")

                        withContext(Dispatchers.Main){
                            //messagelogTextView.text = elapsedTime.toString() +" , "+distanceX.toString()+" , " + distanceY.toString()+" , " + distanceZ.toString()
//                            binding.errorTextView.text = velocityX.toString()+" , " + velocityY.toString()+" , " + velocityZ.toString()
//                            binding.distanceTextView.text =

//                            binding.messagelogTextView.text = deltaTime.toString()
//                            //binding.errorTextView.text = veloChart!!.data.dataSetCount.toString()
//                            binding.distanceTextView.text = distanceZ.toString()

//                            lineChart!!.updateData(lineDataSetX,lineDataSetY,lineDataSetZ)
//                            veloChart!!.updateData(veloDataSetX,veloDataSetY,veloDataSetZ)
                            //distanceChart!!.updateData(distanceDataSetX,distanceDataSetY,distanceDataSetZ)
                        }
                    }
                    lastTimestamp = currentTime

                } catch (e: Exception) {
                    e.printStackTrace()
                }
            }
        } catch (e: Exception) {
            e.printStackTrace()
            Log.d("receive1", "Handled")
        }
    }

    override fun onDataChanged(p0: DataEventBuffer) {

    }

    override fun onCapabilityChanged(p0: CapabilityInfo) {

    }
}
