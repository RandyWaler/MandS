HandCtrl = custAttributes.getdef $.modifiers[1].Hand
attributes Hand
redefine: HandCtrl
--HandCtrl = attributes Hand
(
	parameters main rollout:parmas 
	(
		IKtoFK  type:#float ui:uiIKtoFK
		elbowroll type:#angle ui:uielbowroll
		Fist type:#float ui:uiFist
		SlFist type:#float ui:uiSliFist
		
	)
rollout parmas "�ұۿ���" 
(
	group "�ұ�"
	(
	    spinner uiIKtoFK "IK/FKת����" range:[0,1,0] scale:1
	    button btnFKtoIK " FK ƥ�� IK"
	    button btnIKtoFK " IK ƥ�� FK"
		spinner uielbowroll " �ⲿ��ת��"  range:[-180,180,0] scale:1
	)
	
	group "����"
	(
		
		slider uiSliFist " ��ȭ���� " range:[0,1,0] scale:0.1 tickes:3
		spinner uiFist " ��ȭ " range:[0,1,0] scale:0.1
		
	)
	
	on btnFKtoIK pressed do (
		IKtoFK = 0
		$RArm.transform = $Con_RArmIK00.transform
		$RForearm.transform = $Con_RArmIK01.transform
		$RHand.transform = $Con_RArmIK02.transform
	)
	
	on btnIKtoFK pressed do (
        $Con_RHand.transform = $RHand.transform
	    IKtoFK = 1
    )
)

)
--custAttributes.add $.modifiers[1] HandCtrl