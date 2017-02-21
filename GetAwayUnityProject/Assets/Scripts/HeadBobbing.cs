using UnityEngine;
using System.Collections;

public class HeadBobbing : MonoBehaviour {
	
	
	// drop into the main character's camera
	// editor-accessible vars
	public float bobbingFreq = 1.8f; // in Hz, TODO make it dependent on speed
	private float bobbingFreqCached;
	public float bobbingRatio = 0.08f; // as a factor of character height
	
	// non editor-accessible
	private float phase = Mathf.PI;
	private CharacterMotor characterMotor; // we need to see if the character is walking or jumping (TODO maybe running and sliding)  
	
	private CharacterController characterController;
	
	private float height; //we'll need this for bobbing as a function of height
	
	private float bobbingAmount;
	private float heightDependency;
	private float currentBobbing; // keep track of y axis modification 
	private float bobbingDelta; // how much has bobbing changed in the last frame
	// Keep a mask for the displacement states where isWalking is xxx1 isStepping xx1x and isStopping x1xx
	private int stateMask = 0;
	private const int isWalking = 1; 
	// this next variable can be useful for triggering stepSounds, see getter below
	private const int isStepping = 2; 
	private const int isStopping = 4;
	// need to remember where camera was before we began toying with it. 
	// NOTE this wont work if anything else is moving the camera!
	private Vector3 OriginalCameraLocalPosition;
	
	void  Start (){
		characterMotor = GetComponent<CharacterMotor>();
		characterController = GetComponent<CharacterController>();
		height = characterController.height;
		OriginalCameraLocalPosition = Camera.main.transform.localPosition;
		
		heightDependency = transform.localScale.y * height / 2;
		currentBobbing = 0.0f;
		bobbingFreqCached = bobbingFreq;
	}
	
	void  Update (){ 
		bobbingAmount = bobbingRatio * heightDependency;
		if ( (characterMotor.inputMoveDirection != Vector3.zero) && characterMotor.IsGrounded() ) { 
			UpdatePhaseAndBobbingDelta();
			stateMask = stateMask | isWalking; // isWalking = true;
			stateMask = stateMask & ~isStopping; // isStopping = false;
		} 
		else {
			// bobbingDelta = 0.0f; // see note in our first branch
			if( (stateMask & isWalking) > 0 ){ // if we got here, we're stopping
				stateMask = stateMask | isStopping; // isStopping = true
				UpdatePhaseAndBobbingDelta( (phase > 0) ? 1 : -1 ); // if our phase is less than 0, rewind it
				if ( (stateMask & isStepping) > 0 ) {// we're back down
					Camera.main.transform.localPosition = OriginalCameraLocalPosition; // avoid cumulative errors
					stateMask = stateMask & ~isWalking;
					phase = Mathf.PI; // ready to start again
				}
			}
			else {
				bobbingDelta = 0.0f;
				stateMask = 0; // we're not doing anything
			}
		} 
		Camera.main.transform.Translate(Vector3.up * bobbingDelta, Space.World);    
	}
	
	// by default we add phase but it might be shorter to rewind it when stopping 
	private void  UpdatePhaseAndBobbingDelta (){
		UpdatePhaseAndBobbingDelta( 1.0f );
	}
	
	private void  UpdatePhaseAndBobbingDelta ( float direction  ){
		
		float twoPi = Mathf.PI * 2;
		// bobbing happens here, add one to Sin to avoid going under the terrain
		float previousBobbing = currentBobbing;
		currentBobbing = (Mathf.Cos(phase) + 1.0f) * bobbingAmount;
		bobbingDelta = currentBobbing - previousBobbing;
		// update phase in a frame-independent fashion
		phase = phase + (direction * (twoPi * Time.deltaTime * bobbingFreq));
		// wrap phase
		if (Mathf.Abs(phase) > Mathf.PI) { 
			phase = phase - (direction * twoPi); 
			stateMask = stateMask | isStepping;
		} 
		else
			stateMask = stateMask & ~isStepping;
	}
	
	public bool getIsStepping(){
		return ( (isStepping & stateMask) > 0 );
	}  
}