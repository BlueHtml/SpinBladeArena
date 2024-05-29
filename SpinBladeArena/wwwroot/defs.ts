class Blade {
    angle: number;
    damage: number;
    length: number;

    constructor(raw: BladeDto) {
        this.angle = raw.a;
        this.damage = raw.d;
        this.length = raw.l;
    }
}

class Bonus {
    name: string;
    position: number[];

    constructor(raw: BonusDto) {
        this.name = raw.n;
        this.position = raw.p;
    }

    isUseable(player: Player | null) {
        if (!player) return true;

        if (this.name === BonusNames.BladeCount || this.name === BonusNames.BladeCount3) {
            // ���������ܳ����뾶����8������ȡֵ����Ĭ�ϰ뾶30�����3.75->4�ѵ�������ʱ������
            const maxBladeCount = Math.ceil(player.getSize() / 8);
            return player.blades.length < maxBladeCount;
        }

        if (this.name === BonusNames.BladeLength || this.name === BonusNames.BladeLength20) {
            // �������ܳ�����Ұ뾶��3�������������ȣ������磺Ĭ�ϵ���30����Ұ뾶30����󵶳�90
            const maxBladeLength = player.getSize() * 3;
            return player.blades.some(blade => blade.length < maxBladeLength);
        }

        if (this.name === BonusNames.BladeDamage) {
            // ���˲��ܳ����뾶����15��Ĭ�ϰ뾶30�����2�ˣ�����ʱ�������
            const maxBladeDamage = player.getSize() / 15;
            return player.blades.some(blade => blade.damage < maxBladeDamage);
        }

        if (this.name === BonusNames.BladeSpeed || this.name === BonusNames.BladeSpeed20) {
            // ���ٲ��ܳ�����Ұ뾶��1.5�����������ٶȣ�����ʼ10��ÿ�룬�뾶30�����45��ÿ��
            // ǰ��û�е��ٵ���ʾ����������ж�����ʱ��������
        }

        return true;
    }
}

class BonusNames {
    public static readonly Health: string = "����";
    public static readonly Thin: string = "����";
    public static readonly Speed: string = "����+5";
    public static readonly Speed20: string = "����+20";
    public static readonly BladeCount: string = "����";
    public static readonly BladeCount3: string = "����+3";
    public static readonly BladeLength: string = "����+5";
    public static readonly BladeLength20: string = "����+20";
    public static readonly BladeDamage: string = "����";
    public static readonly BladeSpeed: string = "����+5";
    public static readonly BladeSpeed20: string = "����+20";
    public static readonly Random: string = "���";
}

class Player {
    userId: number;
    userName: string;
    position: number[];
    destination: number[];
    health: number;
    static defaultSize = 30;
    static minSize = 20;
    blades: Blade[];
    score: number;

    constructor(raw: PlayerDto) {
        this.userId = raw.u;
        this.userName = raw.n;
        this.position = raw.p;
        this.destination = raw.d;
        this.health = raw.h;
        this.blades = raw.b.map(bladeRaw => new Blade(bladeRaw));
        this.score = raw.s;
    }

    getSize() {
        const suggestedSize = Player.minSize + this.health;
        return suggestedSize < 0 ? 0 : suggestedSize;
    }

    // ƽ������ƣ�������Ƚ��٣��Ե�ʱ�������˺�����ʱ������ɫΪ��ɫ
    isGoldBlade(blade: Blade) {
        return this.blades.length <= 2 && blade.damage >= 2;
    }
}
// Define the structure for player data
type PlayerDto = {
    u: number;  // Unique user ID
    n: string;  // Username
    s: number;  // Score of the player
    p: number[];  // Current position in the game world, usually an array of x, y, z coordinates
    d: number[];  // Destination position in the game world, similar to 'p'
    h: number;  // Health level of the player
    b: BladeDto[];  // Array of blades the player has
};

// Define the structure for pickable bonuses in the game
type BonusDto = {
    n: string;  // Name of the bonus
    p: number[];  // Position where the bonus is located, typically x, y, z coordinates
};

// Define the structure for a blade, part of a player's weaponry
type BladeDto = {
    a: number;  // Angle at which the blade is being held or used
    l: number;  // Length of the blade
    d: number;  // Damage potential of the blade
};

type PushStateDto = {
    i: number; // frame index
    p: PlayerDto[]; // Array of player data
    b: BonusDto[]; // Array of pickable bonus data
    d: PlayerDto[]; // Array of dead player data
};

declare var initState: PushStateDto;