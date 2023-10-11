# InputHandlerCodeGenerator
Unity의 Input System에 레이어 기능을 추가해서 사용할 수 있는 클래스를 자동으로 생성해주는 코드 생성기

## ver_1  
#### 레이어가 모든 Action을 포함  
* 새로운 레이어를 추가하거나 기존의 레이어를 제거하면 모든 Action이 초기화되거나 복구됨
* 하나의 Action만 관리할 수 없었음
  
***  
  
## ver_2  
#### 각 Action 마다 레이어 적용  
* 하나의 Action에 대해서만 바인딩을 초기화하거나 복구할 수 있음
