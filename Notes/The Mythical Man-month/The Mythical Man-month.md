## 1. 人月神话的核心观点：

弗雷德里克·布鲁克斯强调，软件开发过程中的进度不能仅仅通过增加人力资源来推动，这是因为新加入的成员需要时间来学习和适应项目，同时也会引入更多的沟通成本。这一观点被称为“人月神话”。

- Programming tasks are not partitionable.
- Programming requires communication.

## 2. 布鲁克斯法则：

如果一个软件项目已经延期，那么增加人员将导致进一步的延期。这是因为新成员需要时间来学习代码、了解设计，而现有成员必须花费更多的时间来培训和协调。

## 3. 管理的复杂性：

布鲁克斯解释了软件开发中的复杂性，特别是在大型项目中。他提到了“二次系统效应”，即开发人员在设计第二个系统时倾向于过度设计，从而导致系统变得复杂、不稳定且难以实现。

## 4. 什么样的队伍才是合适的？

小团队固然高效，但是你不能指望一个 20 人的小团队在合理的时间内去开发一套完整的操作系统吧？布鲁克斯在这一章里给出了解决方案：将大项目合理地划分成更小的系统，各个外科手术队伍分别开发这些更小的系统。Harlan Mills 提供了一个解决方案：大型项目的每一个部分由一个团队解决，但是该队伍并不是一拥而上，而是以类似外科手术队伍的方式组建。如今的外科手术已经相当成熟，通常来说，一个外科手术队伍中有五种角色：

- 一位外科医生

- 一位或多位助手

- 一位麻醉师

- 一位器械护士

- 一位巡回护士

其中，主刀的外科医生负责整台手术的操作部分，他必须经验丰富且能力出色。助手是外科医生的后备，他能完成任何一部分工作，但是相对具有较少的经验。麻醉师、护士为外科医生提供必要的帮助。 在系统开发中也可以采用类似的人员结构：

- 程序的总架构师担任外科医生的角色，负责程序的整体架构，并输出产品文档。保障程序的概念完整性，在整个团队中具有权威性。

- 助手需要了解程序的所有代码，负责与架构师讨论程序的结构，但不具有架构师的权威，他充当了外科医生的保险机制。

- 将程序员按照专业化分工，承担自己擅长领域的代码部分，他们创造了团队最有价值的财富 —— 工作产品。

- 测试人员负责编写测试用例，为外科医生搭建测试平台，保障程序的可靠性。

外科手术队伍与传统队伍的区别就在于：传统的团队将工作进行划分，每人负责一部分工作的设计和实现。在外科手术团队中，实际设计完全由外科医生完成，其他人负责填充实现细节与提供必要的帮助。外科手术队伍的好处在于：他减少了沟通的成本，在传统的队伍中大家是平等的，出现观点的差异时，不可避免的需要讨论和妥协，在外科手术队伍中，观点的不一致由外科医生单方面来统一，它保证了程序的概念完整性。

## 5. 巴比伦塔的教训

虽然有充足的人力物力，但由于无法沟通，不得不走向失败的结局。它的教训告诉我们：沟通是合作的必要条件。大型软件项目中也面临这样的情况，因为左手不知道右手在做什么，所以进度灾难、不合理的功能实现、系统缺陷纷纷出现。随着工作的不断进行，许多小组开始慢慢地修改自己程序的功能、规模和速度，当他们组合在一起时，新的程序就显得与原定目标渐行渐远。沟通与交流是成功的关键。这项技能需要管理者仔细考虑，相关经验的积累和能力的提高同软件技术本身一样重要。

## 6. 不断扬弃

大多数软件项目都是先设计算法，再将其应用到即将发布的产品中，并按进度计划将首次开发的产品发布给客户。但多数项目的首次开发产品往往不易使用，可能存在过大、过慢或难以操作等问题，且许多原型设计的缺陷在项目落地后才显露出来。若要开发出更完善、更灵巧的系统，除了重新开始之外别无他法，丢弃旧系统可一步到位也可逐步实现。经验表明，这是必要步骤。

## 7. 磨刀不误砍柴工

好的开发工具和开发环境可以极大地提高开发效率。

## 8. 进度管理和监控的方法

慢性的进度偏离会消磨士气，因此关键是要意识到进度滞后可能产生温水煮青蛙效应并采取预防措施。重大灾害可以应对，但日常的进度落后难以察觉、预防和弥补。对优秀的软件开发团队来说，像棒球队伍一样，进度是不可或缺的要素。为加强进度监控，定义里程碑至关重要。里程碑应是具体的、特定的、可度量的事件，明确界定。明确的里程碑可避免弄虚作假，而模糊的里程碑往往导致与实际情况不符的报告。进度跟踪和管理中，整个团队应及早完成任务，而非拖延至最后一刻。关键链进度管理强调压缩前期开发周期并预留缓冲。进度出现偏差时，项目经理常会掩盖问题并试图赶进度，但这往往无法解决问题根源。老板需明辨状况报告并及早采取措施。老板应鼓励诚实汇报。

## 9. 没有银弹

在未来的十年内，无论是在技术还是管理方法上，都看不出有任何突破性的进步，能够保证在十年内大幅度地提高软件的生产率、可靠性和简洁性。在软件开发行业，我们一直渴求一颗消灭软件复杂度的银弹。其一，计算机技术的发展太快了，为了不被新技术抛弃，工作人员需要不断学习新的知识技能，同时也要应对社会的其他因素带来的软件需求变化。每一次的新技术学习都是一次不小的挑战。其二，软件研发的本身内在特性也制约了软件工程的进展，布鲁克斯用四个词概括了这种特性。

- 复杂性：没有哪两个软件是一模一样的，复杂是软件的根本特性

- 一致性：大型软件开发中，界面、接口常常会不一致，并且随着时间的推移会变得越来越不一致

- 易变性：软件构成的因素随时都在变化

- 不可见性：程序是看不见的，即使用图示方法，也难以充分表现其结构，使得人们沟通面临极大的困难。没有银弹能够解决这些根本困难，

但有一些途径去改善他们：

- 使用新技术，比如高级语言

- 快速开发原型，再做增量开发，而不是采用线性的瀑布模型。

- 聘用人才。寻找卓越的人，并给他们最优厚的待遇。

软件的复杂体现在它是纯思维的产物，是一个纯抽象的概念。具体语法层面上的实现只是软件开发中的次要问题。除非次要问题能占到开发活动的 9/10 以上，否则即使全部次要任务的时间缩减到零，也不会带来生产率数量级上的提高。