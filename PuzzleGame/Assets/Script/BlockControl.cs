﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public enum STEP
    {
        NONE = -1, // 상태 정보 없음.
        IDLE = 0, // 대기 중.
        GRABBED, // 잡혀 있음.
        RELEASED, // 떨어진 순간.
        SLIDE, // 슬라이드 중.
        VACANT, // 소멸 중.
        RESPAWN, // 재생성 중.
        FALL, // 낙하 중.
        LONG_SLIDE, // 크게 슬라이드 중.
        NUM, // 상태가 몇 종류인지 표시.
    };

    public static float COLLISION_SIZE = 1.0f; // 블록의 충돌 크기.
    public static float VANISH_TIME = 3.0f; // 불 붙고 사라질 때까지의 시간.
    public struct iPosition
    { // 그리드에서의 좌표를 나타내는 구조체.
        public int x; // X 좌표.
        public int y; // Y 좌표.
    }
    //public enum CARD
    //{ // 블록 색상.
    //    NONE = -1, // 색 지정 없음.
    //    PINK = 0, // 분홍색.
    //    BLUE, // 파란색.
    //    YELLOW, // 노란색.
    //    GREEN, // 녹색.
    //    MAGENTA, // 마젠타.
    //    ORANGE, // 주황색.
    //    GRAY, // 그레이.
    //    NUM, // 컬러가 몇 종류인지 나타낸다(=7).
    //    FIRST = PINK, // 초기 컬러(분홍색).
    //    LAST = ORANGE, // 최종 컬러(주황색).
    //    NORMAL_COLOR_NUM = GRAY, // 보통 컬러(회색 이외의 색)의 수.
    //};

    public enum CARD
    {
        NONE = -1,
        HEART = 0,
        DIAMOND = 1,
        SPADE = 2,
        CLOBER = 3,
        GRAY = 4, 
        NUM = 5,
        FIRST = HEART, // 초기 머테리얼(하트).
        LAST = CLOBER, // 최종 머테리얼(클로버).
        NORMAL_CARD_NUM = 4
    };

    public enum DIR4
    { // 상하좌우 네 방향.
        NONE = -1, // 방향지정 없음.
        RIGHT, // 우.
        LEFT, // 좌.
        UP, // 상.
        DOWN, // 하.
        NUM, // 방향이 몇 종류 있는지 나타낸다(=4).
    };
    public static int BLOCK_NUM_X = 9;
    // 블록을 배치할 수 있는 X방향 최대수.
    public static int BLOCK_NUM_Y = 9;
    // 블록을 배치할 수 있는 Y방향 최대수.

}

// BlockControl.cs: BlockControl class
// 블록을 조작하는 클래스이다. - 메인
public class BlockControl : MonoBehaviour
{
    // BlockControl.cs: BlockControl class
    public Block.STEP step = Block.STEP.NONE; // 지금 상태.
    public Block.STEP next_step = Block.STEP.NONE; // 다음 상태.
    private Vector3 position_offset_initial = Vector3.zero; // 교체 전 위치.
    public Vector3 position_offset = Vector3.zero; // 교체 후 위치.

    public float vanish_timer = -1.0f;
    public Block.DIR4 slide_dir = Block.DIR4.NONE;
    public float step_timer = 0.0f;

    // Member 변수 추가
    public Material opague_material; // 불투명 머티리얼.
    public Material transparent_material; // 반투명 머티리얼.

    private struct StepFall
    { // Block class가 아님
        public float velocity; // 낙하 속도.
    }
    private StepFall fall;

    public struct countnum
    {
        public int heart;
        public int diamond;
        public int spade;
        public int clober;
    }

    public countnum counting;

    void Start()
    {

        this.setColor(this.color);
        this.next_step = Block.STEP.IDLE; // 다음 블록을 대기중으로.

        this.counting.heart = 0;
        this.counting.diamond = 0;
        this.counting.spade = 0;
        this.counting.clober = 0;
    }

    public Block.CARD color = (Block.CARD)0; // 블록 색.
    public BlockRoot block_root = null; // 블록의 신.
    public Block.iPosition i_pos; // 블록 좌표.

    public Block.CARD c_material = (Block.CARD)0; // 블록 메테리얼


    // 마우스 위치를 바탕으로 슬라이드된 방향을 구한다.
    public Block.DIR4 calcSlideDir(Vector2 mouse_position)
    {
        Block.DIR4 dir = Block.DIR4.NONE;
        // 지정된 mouse_position과 현재 위치의 차를 나타내는 벡터.
        Vector2 v = mouse_position -
        new Vector2(this.transform.position.x, this.transform.position.y);
        // 벡터의 크기가 0.1보다 크면.
        // (그보다 작으면 슬라이드하지 않은 걸로 간주한다).

        if (v.magnitude > 0.1f)
        {
            if (v.y > v.x)
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.UP;
                }
                else
                {
                    dir = Block.DIR4.LEFT;
                }
            }
            else
            {
                if (v.y > -v.x)
                {
                    dir = Block.DIR4.RIGHT;
                }
                else
                {
                    dir = Block.DIR4.DOWN;
                }
            }
        }
        return (dir);
    }

    void Update()
    {
        Vector3 mouse_position; // 마우스 위치.
        this.block_root.unprojectMousePosition( // 마우스 위치 획득.
        out mouse_position, Input.mousePosition);
        // 획득한 마우스 위치를 X와 Y만으로 한다.
        Vector2 mouse_position_xy =
        new Vector2(mouse_position.x, mouse_position.y);
        // '다음 블록' 상태가 '정보 없음' 이외인 동안.
        // ＝'다음 블록' 상태가 변경된 경우.

        if (this.vanish_timer >= 0.0f)
        { // 타이머가 0 이상이면.
            this.vanish_timer -= Time.deltaTime; // 타이머의 값을 줄인다.
            if (this.vanish_timer < 0.0f)
            { // 타이머가 0 미만이면.
                if (this.step != Block.STEP.SLIDE)
                { // 슬라이드 중이 아니라면.
                    this.vanish_timer = -1.0f;
                    this.next_step = Block.STEP.VACANT; // 상태를 ‘소멸 중’으로.
                }
                else
                {
                    this.vanish_timer = 0.0f;
                }
            }
        }

        this.step_timer += Time.deltaTime;
        float slide_time = 0.2f;

        if (this.next_step == Block.STEP.NONE)
        { // '상태 정보 없음'의 경우.
            switch (this.step)
            {
                case Block.STEP.SLIDE:
                    if (this.step_timer >= slide_time)
                    {
                        // 슬라이드 중인 블록이 소멸되면 VACANT(사라진) 상태로 이행.
                        if (this.vanish_timer == 0.0f)
                        {
                            this.next_step = Block.STEP.VACANT;
                            // vanish_timer가 0이 아니면 IDLE(대기) 상태로 이행.
                        }
                        else
                        {
                            this.next_step = Block.STEP.IDLE;
                        }
                    }
                    break;

                case Block.STEP.IDLE:
                    this.GetComponent<Renderer>().enabled = true;
                    break;
                case Block.STEP.FALL:
                    if (this.position_offset.y <= 0.0f)
                    {
                        this.next_step = Block.STEP.IDLE;
                        this.position_offset.y = 0.0f;
                    }
                    break;
            }
        }

        while (this.next_step != Block.STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = Block.STEP.NONE;
            switch (this.step)
            {
                case Block.STEP.IDLE: // '대기' 상태.
                    this.position_offset = Vector3.zero;
                    // 블록 표시 크기를 보통 크기로 한다.
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.GRABBED: // '잡힌' 상태.
                                         // 블록 표시 크기를 크게 한다.
                    this.transform.localScale = Vector3.one * 1.2f;
                    break;
                case Block.STEP.RELEASED: // '떨어져 있는' 상태.
                    this.position_offset = Vector3.zero;
                    // 블록 표시 크기를 보통 사이즈로 한다.
                    this.transform.localScale = Vector3.one * 1.0f;
                    break;
                case Block.STEP.VACANT:
                    this.position_offset = Vector3.zero;
                    this.setVisible(false); // 블록을 표시하지 않게 한다.
                    break;
                case Block.STEP.RESPAWN:
                    // 색을 랜덤하게 선택하여 블록을 그 색으로 설정.
                    //int color_index = Random.Range(
                    //0, (int)Block.COLOR.NORMAL_COLOR_NUM);
                    //this.setColor((Block.COLOR)color_index);

                    int color_index = Random.Range(
                    0, (int)Block.CARD.NORMAL_CARD_NUM);
                    this.setColor((Block.CARD)color_index);

                    this.next_step = Block.STEP.IDLE;
                    break;
                case Block.STEP.FALL:
                    this.setVisible(true); // 블록을 표시.
                    this.fall.velocity = 0.0f; // 낙하 속도 리셋.
                    break;

            }
            this.step_timer = 0.0f;
        }

        switch (this.step)
        {
            case Block.STEP.GRABBED: // 잡힌 상태.
                                     // 잡힌 상태일 때는 항상 슬라이드 방향을 체크.
                this.slide_dir = this.calcSlideDir(mouse_position_xy);
                break;
            case Block.STEP.SLIDE: // 슬라이드(교체) 중.
                                   // 블록을 서서히 이동하는 처리.
                float rate = this.step_timer / slide_time;
                rate = Mathf.Min(rate, 1.0f);
                rate = Mathf.Sin(rate * Mathf.PI / 2.0f);
                this.position_offset = Vector3.Lerp(this.position_offset_initial, Vector3.zero, rate);
                break;

            case Block.STEP.FALL:
                // 속도에 중력의 영향을 부여한다.
                this.fall.velocity += Physics.gravity.y * Time.deltaTime * 0.3f;
                // 세로 방향 위치를 계산.
                this.position_offset.y += this.fall.velocity * Time.deltaTime;
                if (this.position_offset.y < 0.0f)
                { // 다 내려왔다면.
                    this.position_offset.y = 0.0f; // 그 자리에 머무른다.
                }
                break;
        }
        // 그리드 좌표를 실제 좌표(씬의 좌표)로 변환하고.
        // position_offset을 추가한다.
        Vector3 position =
        BlockRoot.calcBlockPosition(this.i_pos) + this.position_offset;
        // 실제 위치를 새로운 위치로 변경한다.
        this.transform.position = position;

        this.setColor(this.color);
        if (this.vanish_timer >= 0.0f)
        {
            // 현재 레벨의 연소시간으로 설정.
            float vanish_time = this.block_root.level_control.getVanishTime();

            // 현재 색과 흰색의 중간 색.
            Color color0 = Color.Lerp(this.GetComponent<Renderer>().material.color,
            Color.white, 0.5f);
            // 현재 색과 검은색의 중간 색.
            Color color1 = Color.Lerp(this.GetComponent<Renderer>().material.color,
            Color.black, 0.5f);
            // 불붙는 연출 시간이 절반을 지났다면.
            if (this.vanish_timer < Block.VANISH_TIME / 2.0f)
            {
                // 투명도(a)를 설정.
                color0.a = this.vanish_timer / (Block.VANISH_TIME / 2.0f);
                color1.a = color0.a;
                // 반투명 머티리얼을 적용.
                this.GetComponent<Renderer>().material = this.transparent_material;
            }
            // vanish_timer가 줄어들수록 1에 가까워진다.
            float rate = 1.0f - this.vanish_timer / Block.VANISH_TIME;
            // 서서히 색을 바꾼다.
            this.GetComponent<Renderer>().material.color = Color.Lerp(color0, color1, rate);
        }

        countBlockDestroy();
    }


    /// ////////////////////////////////////////////////////////////////////////////

    public void beginGrab() // 잡혔을 때 호출한다.
    {
        this.next_step = Block.STEP.GRABBED;
    }
    public void endGrab() // 놓았을 때 호출한다.
    {
        this.next_step = Block.STEP.IDLE;
    }
    public bool isGrabbable() // 잡을 수 있는 상태 인지 판단한다.
    {
        bool is_grabbable = false;
        switch (this.step)
        {
            case Block.STEP.IDLE: // '대기' 상태일 때만.
                is_grabbable = true; // true(잡을 수 있다)를 반환한다.
                break;
        }
        return (is_grabbable);
    }

    // 지정된 마우스 좌표가 자신과 겹치는지 반환한다.
    public bool isContainedPosition(Vector2 position)
    {
        bool ret = false;
        Vector3 center = this.transform.position;
        float h = Block.COLLISION_SIZE / 2.0f;
        do
        {
            // X 좌표가 자신과 겹치지 않으면 break로 루프를 빠져 나간다.
            if (position.x < center.x - h || center.x + h < position.x)
            {
                break;
            }
            // Y 좌표가 자신과 겹치지 않으면 break로 루프를 빠져 나간다.
            if (position.y < center.y - h || center.y + h < position.y)
            {
                break;
            }
            // X 좌표, Y 좌표 모두 겹쳐 있으면 true(겹쳐 있다)를 반환한다.
            ret = true;
        } while (false);
        return (ret);
    }

    /// ////////////////////////////////////////////////////////////////////////////



    // 인수 color의 색으로 블록을 칠한다.
    public void setColor(Block.CARD color)
    {
        this.color = color; // 이번에 지정된 색을 멤버 변수에 보관한다.
        Color color_value; // Color 클래스는 색을 나타낸다.

        Material card_mt;

        switch (this.color)
        { // 칠할 색에 따라서 갈라진다.
            default:
            case Block.CARD.HEART:
                card_mt = Resources.Load("Fire", typeof(Material)) as Material;
                //Material[] mts = renderer.materials;
                //mts[1] = mts;
                //renderer.materials = mts;
                break;
            case Block.CARD.DIAMOND:
                card_mt = Resources.Load("Water", typeof(Material)) as Material;
                break;
            case Block.CARD.SPADE:
                card_mt = Resources.Load("Ground", typeof(Material)) as Material;
                break;
            case Block.CARD.CLOBER:
                card_mt = Resources.Load("Heal", typeof(Material)) as Material;
                break;
            //case Block.CARD.MAGENTA:
            //    color_value = Color.magenta;
            //    break;
            //case Block.CARD.ORANGE: // 이런 컬러들은 유니티에 미리 지정되어있지 않아서 따로 색상 지정해줘야함
            //    color_value = new Color(1.0f, 0.46f, 0.0f);
            //    break;
        }
        // 이 게임 오브젝트의 머티리얼 색상을 변경한다.
        this.GetComponent<Renderer>().material = card_mt;
    }

    public void countBlockDestroy()
    {
        if (this.vanish_timer == 0.0f)
        {
            if (this.color == Block.CARD.HEART)
            {
                this.counting.heart++;
                Debug.Log(this.counting.heart);
            }
            if (this.color == Block.CARD.DIAMOND)
            {
                this.counting.diamond++;
            }
            if (this.color == Block.CARD.SPADE)
            {
                this.counting.spade++;
            }
            if (this.color == Block.CARD.CLOBER)
            {
                this.counting.clober++;
            }
        }
    }

    // 현재 위치와 슬라이드할 곳의 거리가 어느 정도인가 반환한다.
    public float calcDirOffset(Vector2 position, Block.DIR4 dir)
    {
        float offset = 0.0f;
        // 지정된 위치와 블록의 현재 위치의 차를 나타내는 벡터.
        Vector2 v = position - new Vector2(
        this.transform.position.x, this.transform.position.y);
        switch (dir)
        { // 지정된 방향에 따라 갈라진다.
            case Block.DIR4.RIGHT:
                offset = v.x;
                break;
            case Block.DIR4.LEFT:
                offset = -v.x;
                break;
            case Block.DIR4.UP:
                offset = v.y;
                break;
            case Block.DIR4.DOWN:
                offset = -v.y;
                break;
        }
        return (offset);
    }

    // 이동 시작을 알리는 메서드
    public void beginSlide(Vector3 offset)
    {
        this.position_offset_initial = offset;
        this.position_offset = this.position_offset_initial;
        // 상태를 SLIDE로 변경.
        this.next_step = Block.STEP.SLIDE;
    }

    // BlockControl.cs: BlockControl class
    public void toVanishing()
    {
        // '사라질 때까지 걸리는 시간'을 규정값으로 리셋.
        //this.vanish_timer = Block.VANISH_TIME;

        // 현재 레벨의 연소시간으로 설정.
        float vanish_time = this.block_root.level_control.getVanishTime();
        this.vanish_timer = vanish_time;

    }
    public bool isVanishing()
    {
        // vanish_timer가 0보다 크면 true.
        bool is_vanishing = (this.vanish_timer > 0.0f);
        return (is_vanishing);
    }
    public void rewindVanishTimer()
    {
        // '사라질 때까지 걸리는 시간'을 규정값으로 리셋.
        // this.vanish_timer = Block.VANISH_TIME;

        // 현재 레벨의 연소시간으로 설정.
        float vanish_time = this.block_root.level_control.getVanishTime();
        this.vanish_timer = vanish_time;
    }

    public bool isVisible()
    {
        // 그리기 가능(renderer.enabled가 true) 상태라면 표시.
        bool is_visible = this.GetComponent<Renderer>().enabled;
        return (is_visible);
    }
    public void setVisible(bool is_visible)
    {
        // 그리기 가능 설정에 인수를 대입.
        this.GetComponent<Renderer>().enabled = is_visible;
    }
    public bool isIdle()
    {
        bool is_idle = false;
        // 현재 블록 상태가 '대기 중'이고, 다음 블록 상태가 '없음'이면.
        if (this.step == Block.STEP.IDLE && this.next_step == Block.STEP.NONE)
        {
            is_idle = true;
        }
        return (is_idle);
    }

    // 낙하 시작 처리를 한다.
    public void beginFall(BlockControl start)
    {
        this.next_step = Block.STEP.FALL;
        // 지정된 블록에서 좌표를 계산해 낸다.
        this.position_offset.y =
        (float)(start.i_pos.y - this.i_pos.y) * Block.COLLISION_SIZE;
    }
    // 색이 바꿔 낙하 상태로 하고 지정한 위치에 재배치한다.
    public void beginRespawn(int start_ipos_y)
    {
        // 지정 위치까지 y좌표를 이동.
        this.position_offset.y =
        (float)(start_ipos_y - this.i_pos.y) * Block.COLLISION_SIZE;
        this.next_step = Block.STEP.FALL;

        //int color_index =
        //Random.Range((int)Block.COLOR.FIRST, (int)Block.COLOR.LAST + 1);
        //this.setColor((Block.COLOR)color_index);

        // 현재 레벨의 출현 확률을 바탕으로 블록의 색을 결정한다.
        Block.CARD color = this.block_root.selectBlockColor();
        this.setColor(color);
    }

    // 블록이 비표시(그리드상의 위치가 텅 빔)로 되어 있다면 true를 반환한다.
    public bool isVacant()
    {
        bool is_vacant = false;
        if(this.step == Block.STEP.VACANT && this.next_step ==
        Block.STEP.NONE) {
            is_vacant = true;
        }
        return (is_vacant);
    }
    // 교체 중(슬라이드 중)이라면 true를 반환한다.
    public bool isSliding()
    {
        bool is_sliding = (this.position_offset.x != 0.0f);
        return (is_sliding);
    }
}