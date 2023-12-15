/**
 *
 * Copyright 2019-2023 Bharath Vishal G.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 **/

package com.bharathvishal.messagecommunicationusingwearabledatalayer

//import kotlinx.coroutines.DefaultExecutor.thread


import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.google.android.gms.wearable.*
import kotlinx.coroutines.*
import java.util.*


//class MainActivity : AppCompatActivity(),CoroutineScope by MainScope(),
//    DataClient.OnDataChangedListener,
//    MessageClient.OnMessageReceivedListener,
//    CapabilityClient.OnCapabilityChangedListener {
//    var activityContext: Context? = null
//    private val wearableAppCheckPayload = "AppOpenWearable"
//    private val wearableAppCheckPayloadReturnACK = "AppOpenWearableACK"
//    private var wearableDeviceConnected: Boolean = false
//
//    private var currentAckFromWearForAppOpenCheck: String? = null
//    private val APP_OPEN_WEARABLE_PAYLOAD_PATH = "/APP_OPEN_WEARABLE_PAYLOAD"
//
//    private val MESSAGE_ITEM_RECEIVED_PATH: String = "/message-item-received"
//
//    private val TAG_GET_NODES: String = "getnodes1"
//    private val TAG_MESSAGE_RECEIVED: String = "receive1"
//
//    private var messageEvent: MessageEvent? = null
//    private var wearableNodeUri: String? = null
//
//    private lateinit var binding: ActivityMainBinding
//
//    private var lineChart :LineChart? = null
//    private var veloChart :LineChart? = null
//    private var distanceChart :LineChart? = null
//
//    private lateinit var thread : Thread
//
//    private var count = 0.0f
//
//    private var beforeAccX = 0.0f
//    private var beforeAccY = 0.0f
//    private var beforeAccZ = 0.0f
//    private var beforeVeloX = 0.0f
//    private var beforeVeloY= 0.0f
//    private var beforeVeloZ = 0.0f
//    private var beforeDistX = 0.0f
//    private var beforeDistY= 0.0f
//    private var beforeDistZ= 0.0f
//
//    private var accelX : ArrayList<Entry>? = null
//    private var accelY : ArrayList<Entry>? = null
//    private var accelZ : ArrayList<Entry>? = null
//
//    private var veloX : ArrayList<Entry>? = null
//    private var veloY : ArrayList<Entry>? = null
//    private var veloZ : ArrayList<Entry>? = null
//
//    private var distanceX : ArrayList<Entry>? = null
//    private var distanceY : ArrayList<Entry>? = null
//    private var distanceZ : ArrayList<Entry>? = null
//
//    private var lineDataSetX = LineDataSet(accelX,"X")
//    private var lineDataSetY = LineDataSet(accelY,"Y")
//    private var lineDataSetZ = LineDataSet(accelZ,"Z")
//
//    private var veloDataSetX = LineDataSet(veloX,"X")
//    private var veloDataSetY = LineDataSet(veloY,"Y")
//    private var veloDataSetZ = LineDataSet(veloZ,"Z")
//
//    private var distanceDataSetX = LineDataSet(distanceX,"X")
//    private var distanceDataSetY = LineDataSet(distanceY,"Y")
//    private var distanceDataSetZ = LineDataSet(distanceZ,"Z")
//
//    private var startTime : Long = 0
//    private var lastTimestamp = System.currentTimeMillis()
//
//    private var f : DecimalFormat?=null
//
//    private lateinit var unityView:ConstraintLayout
//    private lateinit var lp:ViewGroup.LayoutParams
//
//    @SuppressLint("SetTextI18n")
//    override fun onCreate(savedInstanceState: Bundle?) {
//        super.onCreate(savedInstanceState)
//        f = DecimalFormat("#.#####")
//
//        accelX = ArrayList()
//        accelY = ArrayList()
//        accelZ = ArrayList()
//
//        veloX = ArrayList()
//        veloY = ArrayList()
//        veloZ = ArrayList()
//
//        distanceX = ArrayList()
//        distanceY = ArrayList()
//        distanceZ = ArrayList()
//
//        binding = ActivityMainBinding.inflate(layoutInflater)
//        val view = binding.root
//        setContentView(view)
//        startActivity(Intent(this, UnityHandlerActivity::class.java))
//
//
//        activityContext = this
//        wearableDeviceConnected = false
//
//        binding.checkwearablesButton.setOnClickListener {
//            if (!wearableDeviceConnected) {
//                val tempAct: Activity = activityContext as MainActivity
//                //Couroutine
//                initialiseDevicePairing(tempAct)
//            }
//        }
//
//        unityView = findViewById(R.id.unity)
//        try{
//            val unityPlayer = UnityPlayer(this)
//            val glesMode = unityPlayer.settings.getInt("gles mode",1)
//            val trueColor = false
//            unityPlayer.init(glesMode,trueColor)
//
//            lp = ViewGroup.LayoutParams(800,1200)
//            unityView.addView(unityPlayer.view,0)
//        } catch(e:Exception){
//            Log.e("mainactivity","Error $e")
//        }
//
//
//        //lineChart = findViewById(R.id.lineChart)
//        //veloChart = findViewById(R.id.veloChart)
//        //distanceChart = findViewById(R.id.distanceChart)
//        //val errorLog:TextView = findViewById(R.id.errorTextView)
//
//        //setChart()
//
//        //lineChart!!.invalidate()
//        //veloChart!!.invalidate()
//        //distanceChart!!.invalidate()
//
//
////        binding.sendmessageButton.setOnClickListener {
////            if (wearableDeviceConnected) {
////                if (binding.messagecontentEditText.text!!.isNotEmpty()) {
////
////                    val nodeId: String = messageEvent?.sourceNodeId!!
////                    // Set the data of the message to be the bytes of the Uri.
////                    val payload: ByteArray =
////                        binding.messagecontentEditText.text.toString().toByteArray()
////
////                    // Send the rpc
////                    // Instantiates clients without member variables, as clients are inexpensive to
////                    // create. (They are cached and shared between GoogleApi instances.)
////                    val sendMessageTask =
////                        Wearable.getMessageClient(activityContext!!)
////                            .sendMessage(nodeId, MESSAGE_ITEM_RECEIVED_PATH, payload)
////
////                    sendMessageTask.addOnCompleteListener {
////                        if (it.isSuccessful) {
////                            Log.d("send1", "Message sent successfully")
////                            val sbTemp = StringBuilder()
////                            sbTemp.append("\n")
////                            sbTemp.append(binding.messagecontentEditText.text.toString())
////                            sbTemp.append(" (Sent to Wearable)")
////                            Log.d("receive1", " $sbTemp")
////                            binding.messagelogTextView.append(sbTemp)
////
////                            binding.scrollviewText.requestFocus()
////                            binding.scrollviewText.post {
////                                binding.scrollviewText.scrollTo(0, binding.scrollviewText.bottom)
////                            }
////                        } else {
////                            Log.d("send1", "Message failed.")
////                        }
////                    }
////                } else {
////                    Toast.makeText(
////                        activityContext,
////                        "Message content is empty. Please enter some message and proceed",
////                        Toast.LENGTH_SHORT
////                    ).show()
////                }
////            }
////        }
//    }
//
//
//    @SuppressLint("SetTextI18n")
//    private fun initialiseDevicePairing(tempAct: Activity) {
//        //Coroutine
//        launch(Dispatchers.Default) {
//            var getNodesResBool: BooleanArray? = null
//
//            try {
//                getNodesResBool =
//                    getNodes(tempAct.applicationContext)
//            } catch (e: Exception) {
//                e.printStackTrace()
//            }
//
//            //UI Thread
//            withContext(Dispatchers.Main) {
//                if (getNodesResBool!![0]) {
//                    //if message Acknowlegement Received
//                    if (getNodesResBool[1]) {
//                        Toast.makeText(
//                            activityContext,
//                            "Wearable device paired and app is open. Tap the \"Send Message to Wearable\" button to send the message to your wearable device.",
//                            Toast.LENGTH_LONG
//                        ).show()
//                        binding.deviceconnectionStatusTv.text =
//                            "Wearable device paired and app is open."
//                        binding.deviceconnectionStatusTv.visibility = View.VISIBLE
//                        wearableDeviceConnected = true
//                        //binding.sendmessageButton.visibility = View.VISIBLE
//                    } else {
//                        Toast.makeText(
//                            activityContext,
//                            "A wearable device is paired but the wearable app on your watch isn't open. Launch the wearable app and try again.",
//                            Toast.LENGTH_LONG
//                        ).show()
//                        binding.deviceconnectionStatusTv.text =
//                            "Wearable device paired but app isn't open."
//                        binding.deviceconnectionStatusTv.visibility = View.VISIBLE
//                        wearableDeviceConnected = false
//                        //binding.sendmessageButton.visibility = View.GONE
//                    }
//                } else {
//                    Toast.makeText(
//                        activityContext,
//                        "No wearable device paired. Pair a wearable device to your phone using the Wear OS app and try again.",
//                        Toast.LENGTH_LONG
//                    ).show()
//                    binding.deviceconnectionStatusTv.text =
//                        "Wearable device not paired and connected."
//                    binding.deviceconnectionStatusTv.visibility = View.VISIBLE
//                    wearableDeviceConnected = false
//                    //binding.sendmessageButton.visibility = View.GONE
//                }
//            }
//        }
//    }
//
//
//    private fun getNodes(context: Context): BooleanArray {
//        val nodeResults = HashSet<String>()
//        val resBool = BooleanArray(2)
//        resBool[0] = false //nodePresent
//        resBool[1] = false //wearableReturnAckReceived
//        val nodeListTask =
//            Wearable.getNodeClient(context).connectedNodes
//        try {
//            // Block on a task and get the result synchronously (because this is on a background thread).
//            val nodes =
//                Tasks.await(
//                    nodeListTask
//                )
//            Log.e(TAG_GET_NODES, "Task fetched nodes")
//            for (node in nodes) {
//                Log.e(TAG_GET_NODES, "inside loop")
//                nodeResults.add(node.id)
//                try {
//                    val nodeId = node.id
//                    // Set the data of the message to be the bytes of the Uri.
//                    val payload: ByteArray = wearableAppCheckPayload.toByteArray()
//                    // Send the rpc
//                    // Instantiates clients without member variables, as clients are inexpensive to
//                    // create. (They are cached and shared between GoogleApi instances.)
//                    val sendMessageTask =
//                        Wearable.getMessageClient(context)
//                            .sendMessage(nodeId, APP_OPEN_WEARABLE_PAYLOAD_PATH, payload)
//                    try {
//                        // Block on a task and get the result synchronously (because this is on a background thread).
//                        val result = Tasks.await(sendMessageTask)
//                        Log.d(TAG_GET_NODES, "send message result : $result")
//                        resBool[0] = true
//
//                        //Wait for 700 ms/0.7 sec for the acknowledgement message
//                        //Wait 1
//                        if (currentAckFromWearForAppOpenCheck != wearableAppCheckPayloadReturnACK) {
//                            Thread.sleep(100)
//                            Log.d(TAG_GET_NODES, "ACK thread sleep 1")
//                        }
//                        if (currentAckFromWearForAppOpenCheck == wearableAppCheckPayloadReturnACK) {
//                            resBool[1] = true
//                            return resBool
//                        }
//                        //Wait 2
//                        if (currentAckFromWearForAppOpenCheck != wearableAppCheckPayloadReturnACK) {
//                            Thread.sleep(250)
//                            Log.d(TAG_GET_NODES, "ACK thread sleep 2")
//                        }
//                        if (currentAckFromWearForAppOpenCheck == wearableAppCheckPayloadReturnACK) {
//                            resBool[1] = true
//                            return resBool
//                        }
//                        //Wait 3
//                        if (currentAckFromWearForAppOpenCheck != wearableAppCheckPayloadReturnACK) {
//                            Thread.sleep(350)
//                            Log.d(TAG_GET_NODES, "ACK thread sleep 5")
//                        }
//                        if (currentAckFromWearForAppOpenCheck == wearableAppCheckPayloadReturnACK) {
//                            resBool[1] = true
//                            return resBool
//                        }
//                        resBool[1] = false
//                        Log.d(
//                            TAG_GET_NODES,
//                            "ACK thread timeout, no message received from the wearable "
//                        )
//                    } catch (exception: Exception) {
//                        exception.printStackTrace()
//                    }
//                } catch (e1: Exception) {
//                    Log.d(TAG_GET_NODES, "send message exception")
//                    e1.printStackTrace()
//                }
//            } //end of for loop
//        } catch (exception: Exception) {
//            Log.e(TAG_GET_NODES, "Task failed: $exception")
//            exception.printStackTrace()
//        }
//        return resBool
//    }
//
//
//    override fun onDataChanged(p0: DataEventBuffer) {
//    }
//
//    @SuppressLint("SetTextI18n")
//    override fun onMessageReceived(p0: MessageEvent) {
//        try {
//            val s =
//                String(p0.data, StandardCharsets.UTF_8)
//            val messageEventPath: String = p0.path
//            Log.d(
//                TAG_MESSAGE_RECEIVED,
//                "onMessageReceived() Received a message from watch:"
//                        + p0.requestId
//                        + " "
//                        + messageEventPath
//                        + " "
//                        + s
//            )
//            if (messageEventPath == APP_OPEN_WEARABLE_PAYLOAD_PATH) {
//                currentAckFromWearForAppOpenCheck = s
//                Log.d(
//                    TAG_MESSAGE_RECEIVED,
//                    "Received acknowledgement message that app is open in wear"
//                )
//
//                val sbTemp = StringBuilder()
//                sbTemp.append(binding.messagelogTextView.text.toString())
//                sbTemp.append("\nWearable device connected.")
//                Log.d("receive1", " $sbTemp")
//                binding.messagelogTextView.text = sbTemp
//                binding.textInputLayout.visibility = View.VISIBLE
//
//                binding.checkwearablesButton.visibility = View.GONE
//                messageEvent = p0
//                wearableNodeUri = p0.sourceNodeId
//            } else if (messageEventPath.isNotEmpty() && messageEventPath == MESSAGE_ITEM_RECEIVED_PATH) {
//
//                try {
//                    binding.messagelogTextView.visibility = View.VISIBLE
//                    binding.textInputLayout.visibility = View.VISIBLE
//                    //binding.sendmessageButton.visibility = View.VISIBLE
//
//                    //시간 측정 함수
//                    if(startTime == 0L){
//                        //첫 데이터 받을때 설정
//                        startTime = System.currentTimeMillis()
//                    }
//                    //데이터를 받은 시간 설정
//                    val currentTime = System.currentTimeMillis()
//                    val elapsedTime = (currentTime - startTime)
//                    val deltaTime = (currentTime - lastTimestamp) / 1000.0
//
//
//
//                    lifecycleScope.launch(Dispatchers.IO) {
//                        //가속도데이터 xyz 분류
//                        var s_arr : List<String> = s.split("\\s".toRegex())
//                        Log.d("1",s_arr.toString())
//                        var charX = s_arr[0]
//                        var charY = s_arr[1]
//                        var charZ = s_arr[2]
//
//                        var velocityX = 0.0f
//                        var velocityY = 0.0f
//                        var velocityZ = 0.0f
//
//                        var distanceX = 0.0f
//                        var distanceY = 0.0f
//                        var distanceZ = 0.0f
//
//                        velocityX = beforeVeloX + charX.toFloat() * deltaTime.toFloat()
//                        velocityY = beforeVeloY + charY.toFloat() * deltaTime.toFloat()
//                        velocityZ = beforeVeloZ + charZ.toFloat() * deltaTime.toFloat()
//
//                        distanceX = beforeDistX + velocityX * deltaTime.toFloat()
//                        distanceY = beforeDistY + velocityY * deltaTime.toFloat()
//                        distanceZ = beforeDistZ + velocityZ * deltaTime.toFloat()
//
//                        beforeAccX = charX.toFloat()
//                        beforeAccY = charY.toFloat()
//                        beforeAccZ = charZ.toFloat()
//
//                        beforeVeloX = velocityX
//                        beforeVeloY = velocityY
//                        beforeVeloZ = velocityZ
//
//                        val sbTemp = StringBuilder()
//                        sbTemp.append("\n")
//                        sbTemp.append(s)
//                        sbTemp.append(" - (Received from wearable)")
//                        Log.d("receive1", " $sbTemp")
//
//                        lineDataSetX.addEntry(Entry(elapsedTime.toFloat(),charX.toFloat()))
//                        lineDataSetY.addEntry(Entry(elapsedTime.toFloat(),charY.toFloat()))
//                        lineDataSetZ.addEntry(Entry(elapsedTime.toFloat(),charZ.toFloat()))
//
//                        veloDataSetX.addEntry(Entry(elapsedTime.toFloat(), velocityX))
//                        veloDataSetY.addEntry(Entry(elapsedTime.toFloat(), velocityY))
//                        veloDataSetZ.addEntry(Entry(elapsedTime.toFloat(), velocityZ))
//
//                        distanceDataSetX.addEntry(Entry(elapsedTime.toFloat(),distanceX))
//                        distanceDataSetY.addEntry(Entry(elapsedTime.toFloat(), distanceY))
//                        distanceDataSetZ.addEntry(Entry(elapsedTime.toFloat(), distanceZ))
//                        withContext(Dispatchers.Main){
////                            binding.messagelogTextView.text = s_arr.toString()
////                            binding.errorTextView.text = velocityX.toString()+" , " + velocityY.toString()+" , " + velocityZ.toString()
////                            binding.distanceTextView.text = distanceX.toString()+" , " + distanceY.toString()+" , " + distanceZ.toString()
//
//                            binding.messagelogTextView.text = deltaTime.toString()
//                            //binding.errorTextView.text = veloChart!!.data.dataSetCount.toString()
//                            binding.distanceTextView.text = distanceZ.toString()
//
////                            lineChart!!.updateData(lineDataSetX,lineDataSetY,lineDataSetZ)
////                            veloChart!!.updateData(veloDataSetX,veloDataSetY,veloDataSetZ)
//                            distanceChart!!.updateData(distanceDataSetX,distanceDataSetY,distanceDataSetZ)
//                        }
//                    }
//                    lastTimestamp = currentTime
//                   // binding.errorTextView.text = accelX.toString()
//
//
//                    Log.d("1","before runnable")
////                    if (thread != null) thread.interrupt()
////                    val runnable = java.lang.Runnable { addEntry(charX.toDouble(), charY.toDouble(), charZ.toDouble()) }
////                    runOnUiThread(
////                        runnable
////                    )
//                    //addEntry(charX.toDouble(), charY.toDouble(), charZ.toDouble())
//                    Log.d("1","after runnable")
////                    val entries = ArrayList<Entry>()
////                    entries.add(Entry(count,count))
////
//                    count++
////                    val dataset = LineDataSet(entries,"")
////                    val lineData = LineData(dataset)
////
////                    lineChart!!.data = lineData
//
////                    binding.scrollviewText.requestFocus()
////                    binding.scrollviewText.post {
////                        binding.scrollviewText.scrollTo(0, binding.scrollviewText.bottom)
////                    }
//                } catch (e: Exception) {
//                    e.printStackTrace()
//                }
//            }
//        } catch (e: Exception) {
//            e.printStackTrace()
//            Log.d("receive1", "Handled")
//        }
//    }
//
//    private fun LineChart.updateData(x:LineDataSet,y:LineDataSet,z:LineDataSet){
//
//        val data : LineData = this.data
//
//        data.apply {
//            addDataSet(x)
//            addDataSet(y)
//            addDataSet(z)
//        }
//        this.invalidate()
//    }
//
//    override fun onCapabilityChanged(p0: CapabilityInfo) {
//    }
//
//
//    override fun onPause() {
//        super.onPause()
//        try {
//            Wearable.getDataClient(activityContext!!).removeListener(this)
//            Wearable.getMessageClient(activityContext!!).removeListener(this)
//            Wearable.getCapabilityClient(activityContext!!).removeListener(this)
//        } catch (e: Exception) {
//            e.printStackTrace()
//        }
//    }
//
//
//    override fun onResume() {
//        super.onResume()
//        try {
//            Wearable.getDataClient(activityContext!!).addListener(this)
//            Wearable.getMessageClient(activityContext!!).addListener(this)
//            Wearable.getCapabilityClient(activityContext!!)
//                .addListener(this, Uri.parse("wear://"), CapabilityClient.FILTER_REACHABLE)
//        } catch (e: Exception) {
//            e.printStackTrace()
//        }
//    }
//
//    private fun LineChart.setChart(){
//        val xAxis : XAxis = this.xAxis
//        xAxis.apply {
//            position = XAxis.XAxisPosition.BOTTOM
//            textSize = 10f
//            setDrawGridLines(false)
//            granularity = 1f
//            isGranularityEnabled = true
//        }
//        this.apply {
//            isAutoScaleMinMaxEnabled = true
//            axisRight.isEnabled = false
//            axisLeft.axisMaximum = 0.2f
//            legend.apply {
//                textSize = 15f
//                verticalAlignment = Legend.LegendVerticalAlignment.TOP
//                horizontalAlignment = Legend.LegendHorizontalAlignment.CENTER
//                orientation = Legend.LegendOrientation.HORIZONTAL
//                setDrawInside(false)
//            }
//        }
//
//        val entries: ArrayList<Entry>? =null
//        entries?.add(Entry(0.0f,0.0f))
//        val dataset = LineDataSet(entries,"")
//        val lineData = LineData(dataset)
//
//        this.data = lineData
//    }
//
//    private fun setChart(){
//
//
//        //lineChart!!.setChart()
//        //veloChart!!.setChart()
//        distanceChart!!.setChart()
//
//    }
//
//    private fun createSet() : LineDataSet{
//        val set = LineDataSet(null, "가속도")
//        set.apply {
//            axisDependency = YAxis.AxisDependency.LEFT
//            color = 255
//            setCircleColor(255)
//            valueTextSize = 10f
//            lineWidth = 2f
//            circleRadius = 3f
//            fillAlpha = 0
//            fillColor = 255
//            //highLightColor
//            setDrawValues(true)
//        }
//        return set
//    }
//

//
//}
class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        startActivity(Intent(this, UnityHandlerActivity::class.java))
    }
}