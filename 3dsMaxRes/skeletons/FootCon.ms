RLegCtrl = custAttributes.getdef $.modifiers[1].RLeg
attributes RLeg
redefine: RLegCtrl
--RLegCtrl = attributes RLeg
(
	parameters main rollout:parmas 
	(
		IKtoFK  type:#float ui:uiIKtoFK
		IKStretch  type:#float ui:uiIKStretch
		footroll type:#angle ui:uifootroll
		toeroll type:#angle ui:uitoeroll
		
	)
rollout parmas "���ȿ���" 
(
	group "����"
	(
	    spinner uiIKtoFK "IK/FKת����" range:[0,1,0] scale:1
	    spinner uiIKStretch "IK���죺" range:[0,1,0] scale:0.1
	    button btnFKtoIK " FK ƥ�� IK"
	    button btnIKtoFK " IK ƥ�� FK"
	)
	
	group "�ҽ�"
	(
	    spinner uifootroll " �Ų���ת��" range:[-45,90,0] scale:1
		spinner uitoeroll " �ż���ת��" range:[-90,90,0] scale:1
			
	)
	
	on btnFKtoIK pressed do (
		IKtoFK = 0
		$RThigh.transform = $Con_RLegIK00.transform
		$RCalf.transform = $Con_RLegIK01.transform
		$RFoot.transform = $Con_RLegIK02.transform
		$RToes.transform = $Con_RLegIK03.transform
	)
	
	on btnIKtoFK pressed do (
        $Con_RFoot.transform = $RFoot.transform
	    IKtoFK = 1
    )
)

)
--custAttributes.add $.modifiers[1] RLegCtrl